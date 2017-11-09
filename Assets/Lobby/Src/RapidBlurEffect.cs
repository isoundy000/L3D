using UnityEngine;
using System.Collections;

//�����ڱ༭ģʽ��Ҳִ�иýű�  
[ExecuteInEditMode]
//���ѡ��˵���  
[AddComponentMenu("Learning Unity Shader/Lecture 15/RapidBlurEffect")]
public class RapidBlurEffect : MonoBehaviour
{
    //-------------------������������-------------------  
    #region Variables  

    //ָ��Shader����  
    private string ShaderName = "Learning Unity Shader/Lecture 15/RapidBlurEffect";

    //��ɫ���Ͳ���ʵ��  
    public Shader CurShader;
    private Material CurMaterial;

    //�������ڵ��ڲ������м����  
    public static int ChangeValue;
    public static float ChangeValue2;
    public static int ChangeValue3;

    //����������  
    [Range(0, 6), Tooltip("[����������]���²����Ĵ�������ֵԽ��,��������Խ��,��Ҫ��������ص�Խ��,�����ٶ�Խ�졣")]
    public int DownSampleNum = 2;
    //ģ����ɢ��  
    [Range(0.0f, 20.0f), Tooltip("[ģ����ɢ��]���и�˹ģ��ʱ���������ص�ļ������ֵԽ���������ؼ��ԽԶ��ͼ��Խģ�����������ֵ�ᵼ��ʧ�档")]
    public float BlurSpreadSize = 3.0f;
    //��������  
    [Range(0, 8), Tooltip("[��������]��ֵԽ��,��ģ�������ĵ�������Խ�࣬ģ��Ч��Խ�ã�������Խ��")]
    public int BlurIterations = 3;

    #endregion

    //-------------------------���ʵ�get&set----------------------------  
    #region MaterialGetAndSet  
    Material material
    {
        get
        {
            if (CurMaterial == null)
            {
                CurMaterial = new Material(CurShader);
                CurMaterial.hideFlags = HideFlags.HideAndDontSave;
            }
            return CurMaterial;
        }
    }
    #endregion

    #region Functions  
    //-----------------------------------------��Start()������---------------------------------------------    
    // ˵�����˺�������Update������һ�α�����ǰ������  
    //--------------------------------------------------------------------------------------------------------  
    void Start()
    {
        //���θ�ֵ  
        ChangeValue = DownSampleNum;
        ChangeValue2 = BlurSpreadSize;
        ChangeValue3 = BlurIterations;

        //�ҵ���ǰ��Shader�ļ�  
        CurShader = Shader.Find(ShaderName);

        //�жϵ�ǰ�豸�Ƿ�֧����Ļ��Ч  
        if (!SystemInfo.supportsImageEffects)
        {
            enabled = false;
            return;
        }
    }

    //-------------------------------------��OnRenderImage()������------------------------------------    
    // ˵�����˺����ڵ����������ȾͼƬ�󱻵��ã�������ȾͼƬ����Ч��  
    //--------------------------------------------------------------------------------------------------------  
    void OnRenderImage(RenderTexture sourceTexture, RenderTexture destTexture)
    {
        //��ɫ��ʵ����Ϊ�գ��ͽ��в�������  
        if (CurShader != null)
        {
            //��0������׼��  
            //�������²����Ĵ���ȷ�����ϵ�������ڿ��ƽ��������������صļ��  
            float widthMod = 1.0f / (1.0f * (1 << DownSampleNum));
            //Shader�Ľ�����������ֵ  
            material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod);
            //������Ⱦģʽ��˫����  
            sourceTexture.filterMode = FilterMode.Bilinear;
            //ͨ�����ƣ�׼�����������ֵ  
            int renderWidth = sourceTexture.width >> DownSampleNum;
            int renderHeight = sourceTexture.height >> DownSampleNum;

            // ��1������Shader��ͨ��0�����ڽ����� ||Pass 0,for down sample  
            //׼��һ������renderBuffer������׼�������������  
            RenderTexture renderBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
            //������Ⱦģʽ��˫����  
            renderBuffer.filterMode = FilterMode.Bilinear;
            //����sourceTexture�е���Ⱦ���ݵ�renderBuffer,��������ָ����pass0����������  
            Graphics.Blit(sourceTexture, renderBuffer, material, 0);

            //��2������BlurIterations��������������������ָ�������ĵ�������  
            for (int i = 0; i < BlurIterations; i++)
            {
                //��2.1��Shader������ֵ  
                //����ƫ��������  
                float iterationOffs = (i * 1.0f);
                //Shader�Ľ�����������ֵ  
                material.SetFloat("_DownSampleValue", BlurSpreadSize * widthMod + iterationOffs);

                // ��2.2������Shader��ͨ��1����ֱ����ģ������ || Pass1,for vertical blur  
                // ����һ����ʱ��Ⱦ�Ļ���tempBuffer  
                RenderTexture tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                // ����renderBuffer�е���Ⱦ���ݵ�tempBuffer,��������ָ����pass1����������  
                Graphics.Blit(renderBuffer, tempBuffer, material, 1);
                //  ���renderBuffer  
                RenderTexture.ReleaseTemporary(renderBuffer);
                // ��tempBuffer����renderBuffer����ʱrenderBuffer����pass0��pass1�������Ѿ�׼����  
                renderBuffer = tempBuffer;

                // ��2.3������Shader��ͨ��2����ֱ����ģ������ || Pass2,for horizontal blur  
                // ��ȡ��ʱ��Ⱦ����  
                tempBuffer = RenderTexture.GetTemporary(renderWidth, renderHeight, 0, sourceTexture.format);
                // ����renderBuffer�е���Ⱦ���ݵ�tempBuffer,��������ָ����pass2����������  
                Graphics.Blit(renderBuffer, tempBuffer, CurMaterial, 2);

                //��2.4���õ�pass0��pass1��pass2�����ݶ��Ѿ�׼���õ�renderBuffer  
                // �ٴ����renderBuffer  
                RenderTexture.ReleaseTemporary(renderBuffer);
                // �ٴν�tempBuffer����renderBuffer����ʱrenderBuffer����pass0��pass1��pass2�����ݶ��Ѿ�׼����  
                renderBuffer = tempBuffer;
            }

            //�������յ�renderBuffer��Ŀ����������������ͨ����������Ļ  
            Graphics.Blit(renderBuffer, destTexture);
            //���renderBuffer  
            RenderTexture.ReleaseTemporary(renderBuffer);

        }

        //��ɫ��ʵ��Ϊ�գ�ֱ�ӿ�����Ļ�ϵ�Ч�������������û��ʵ����Ļ��Ч��  
        else
        {
            //ֱ�ӿ���Դ����Ŀ����Ⱦ����  
            Graphics.Blit(sourceTexture, destTexture);
        }
    }


    //-----------------------------------------��OnValidate()������--------------------------------------    
    // ˵�����˺����ڱ༭���иýű���ĳ��ֵ�����˸ı�󱻵���  
    //--------------------------------------------------------------------------------------------------------  
    void OnValidate()
    {
        //���༭���е�ֵ��ֵ������ȷ���ڱ༭����ֵ�ĸı������ý����Ч  
        ChangeValue = DownSampleNum;
        ChangeValue2 = BlurSpreadSize;
        ChangeValue3 = BlurIterations;
    }

    //-----------------------------------------��Update()������--------------------------------------    
    // ˵�����˺���ÿ֡���ᱻ����  
    //--------------------------------------------------------------------------------------------------------  
    void Update()
    {
        //�����������У����и�ֵ  
        if (Application.isPlaying)
        {
            //��ֵ  
            DownSampleNum = ChangeValue;
            BlurSpreadSize = ChangeValue2;
            BlurIterations = ChangeValue3;
        }
        //������û�������У�ȥѰ�Ҷ�Ӧ��Shader�ļ�  
#if UNITY_EDITOR
        if (Application.isPlaying != true)
        {
            CurShader = Shader.Find(ShaderName);
        }
#endif

    }

    //-----------------------------------------��OnDisable()������---------------------------------------    
    // ˵�����������Ϊ�����û�Ǽ���״̬ʱ�˺����㱻����    
    //--------------------------------------------------------------------------------------------------------  
    void OnDisable()
    {
        if (CurMaterial)
        {
            //�������ٲ���ʵ��  
            DestroyImmediate(CurMaterial);
        }

    }

    #endregion

}