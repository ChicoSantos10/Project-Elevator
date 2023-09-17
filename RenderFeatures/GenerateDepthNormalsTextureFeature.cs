using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace RenderFeatures
{
    public class GenerateDepthNormalsTextureFeature : ScriptableRendererFeature
    {
        class GenerateDepthNormalsPass : ScriptableRenderPass
        {
            const int DepthBufferBits = 32;
            RenderTargetHandle _depthAttachmentHandle;
            internal RenderTextureDescriptor descriptor { get; private set; }

            Material _depthNormalsMaterial;
            FilteringSettings _filteringSettings;
            FilteringSettings _stencilFilteringSettings;

            RenderStateBlock _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);

            const string ProfilerTag = "DepthNormals Prepass";
            ProfilingSampler _profilingSampler = new ProfilingSampler(ProfilerTag);

            static readonly List<ShaderTagId> ShaderTagId = new List<ShaderTagId>()
            {
                new ShaderTagId("DepthOnly")
            };

            static readonly List<ShaderTagId> ShaderTagIds = new List<ShaderTagId>()
            {
                new ShaderTagId("SRPDefaultUnlit"),
                new ShaderTagId("UniversalForward"),
                new ShaderTagId("UniversalForwardOnly"),
                new ShaderTagId("LightweightForward"),
            };

            public GenerateDepthNormalsPass(RenderQueueRange renderQueueRange, LayerMask layerMask, Material material)
            {
                _filteringSettings = new FilteringSettings(renderQueueRange, layerMask);
                _stencilFilteringSettings = new FilteringSettings(RenderQueueRange.opaque, LayerMask.GetMask("Mask"));
                _depthNormalsMaterial = material;
            }

            public void Setup(RenderTextureDescriptor baseDescriptor, RenderTargetHandle depthAttachmentHandle)
            {
                _depthAttachmentHandle = depthAttachmentHandle;
                baseDescriptor.colorFormat = RenderTextureFormat.ARGB32;
                baseDescriptor.depthBufferBits = DepthBufferBits;
                baseDescriptor.stencilFormat = GraphicsFormat.R8_UInt;
                descriptor = baseDescriptor;
            }

            // This method is called before executing the render pass.
            // It can be used to configure render targets and their clear state. Also to create temporary render target textures.
            // When empty this render pass will render to the active camera render target.
            // You should never call CommandBuffer.SetRenderTarget. Instead call <c>ConfigureTarget</c> and <c>ConfigureClear</c>.
            // The render pipeline will ensure target setup and clearing happens in an performance manner.
            public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
            {
                cmd.GetTemporaryRT(_depthAttachmentHandle.id, descriptor, FilterMode.Point);
                ConfigureTarget(_depthAttachmentHandle.Identifier());
                ConfigureClear(ClearFlag.All, Color.black);
            }

            // Here you can implement the rendering logic.
            // Use <c>ScriptableRenderContext</c> to issue drawing commands or execute command buffers
            // https://docs.unity3d.com/ScriptReference/Rendering.ScriptableRenderContext.html
            // You don't have to call ScriptableRenderContext.submit, the render pipeline will call it at specific points in the pipeline.
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                // First we set the stencil buffer
                ExecuteStencilMask(context, ref renderingData);

                // The we render the opaques
                ExecuteNormalsPass(context, ref renderingData);
            }

            void ExecuteNormalsPass(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                CommandBuffer cmd = CommandBufferPool.Get();

                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    SortingCriteria sortFlags = renderingData.cameraData.defaultOpaqueSortFlags;
                    DrawingSettings drawSettings = CreateDrawingSettings(ShaderTagIds, ref renderingData, sortFlags);
                    drawSettings.perObjectData = PerObjectData.None;

                    ref CameraData cameraData = ref renderingData.cameraData;
                    Camera camera = cameraData.camera;
                    if (cameraData.xrRendering)
                        context.StartMultiEye(camera);

                    drawSettings.overrideMaterial = _depthNormalsMaterial;

                    FilteringSettings settings = _filteringSettings;
                    
                    for (int i = 1; i < 5; i++)
                    {
                        SetStencilState(i);
                        settings.layerMask = LayerMask.GetMask("Hidden_" + i);
                        context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref settings,
                            ref _renderStateBlock);
                    }
                    
                    context.DrawRenderers(renderingData.cullResults, ref drawSettings, ref _filteringSettings);
                    
                    cmd.SetGlobalTexture("_CameraDepthNormalsTexture", _depthAttachmentHandle.id);

                    Matrix4x4 clipToView = GL.GetGPUProjectionMatrix(camera.projectionMatrix, true).inverse;
                    cmd.SetGlobalMatrix("_ClipToView", clipToView);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            void ExecuteStencilMask(ScriptableRenderContext context, ref RenderingData renderingData)
            {
                SortingCriteria sortingCriteria = renderingData.cameraData.defaultOpaqueSortFlags;
                DrawingSettings settings = CreateDrawingSettings(ShaderTagIds, ref renderingData, sortingCriteria);
                //settings.overrideMaterial = _depthNormalsMaterial;

                // StencilState stencilState = StencilState.defaultValue;
                // stencilState.enabled = true;
                // stencilState.SetCompareFunction(CompareFunction.Always);
                // stencilState.SetPassOperation(StencilOp.Replace);
                // //stencilState.SetFailOperation(failOp);
                // //stencilState.SetZFailOperation(zFailOp);
                //
                // _renderStateBlock.mask |= RenderStateMask.Stencil;
                // _renderStateBlock.stencilReference = 1;
                // _renderStateBlock.stencilState = stencilState;

                CommandBuffer cmd = CommandBufferPool.Get();
                using (new ProfilingScope(cmd, _profilingSampler))
                {
                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    context.DrawRenderers(renderingData.cullResults, ref settings, ref _stencilFilteringSettings);
                    //cmd.SetGlobalTexture("_CameraDepthNormalsTexture", _depthAttachmentHandle.id);
                }

                context.ExecuteCommandBuffer(cmd);
                CommandBufferPool.Release(cmd);
            }

            /// Cleanup any allocated resources that were created during the execution of this render pass.
            public override void FrameCleanup(CommandBuffer cmd)
            {
                if (_depthAttachmentHandle == RenderTargetHandle.CameraTarget)
                    return;

                cmd.ReleaseTemporaryRT(_depthAttachmentHandle.id);
                _depthAttachmentHandle = RenderTargetHandle.CameraTarget;
            }

            public void SetStencilState(int reference)
            {
                StencilState stencilState = StencilState.defaultValue;
                stencilState.enabled = true;
                stencilState.SetCompareFunction(CompareFunction.Equal);
                //stencilState.SetPassOperation();
                //stencilState.SetFailOperation(failOp);
                //stencilState.SetZFailOperation(zFailOp);

                _renderStateBlock.mask |= RenderStateMask.Stencil;
                _renderStateBlock.stencilReference = reference;
                _renderStateBlock.stencilState = stencilState;
            }
        }

        GenerateDepthNormalsPass _depthNormalsPass;

        RenderTargetHandle _depthNormalsTexture;
        Material _depthNormalsMaterial;
        [SerializeField] LayerMask layer;

        public override void Create()
        {
            const RenderPassEvent passEvent = RenderPassEvent.BeforeRenderingPostProcessing;

            _depthNormalsMaterial = CoreUtils.CreateEngineMaterial("Hidden/Internal-DepthNormalsTexture");

            _depthNormalsPass = new GenerateDepthNormalsPass(RenderQueueRange.opaque, layer, _depthNormalsMaterial)
            {
                renderPassEvent = passEvent
            };
            //_depthNormalsPass.SetStencilState(3);

            _depthNormalsTexture.Init("_CameraDepthNormalsTexture");
        }

        // Here you can inject one or multiple render passes in the renderer.
        // This method is called when setting up the renderer once per-camera.
        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _depthNormalsPass.Setup(renderingData.cameraData.cameraTargetDescriptor, _depthNormalsTexture);
            renderer.EnqueuePass(_depthNormalsPass);
        }
    }
}