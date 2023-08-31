using UnityEngine;
using UnityEngine.Rendering;

public class PolygonPaintManager : Singleton<PolygonPaintManager>
{
    public Shader texturePaint;
    CommandBuffer command;
    private Material paintMaterial;
    int colorID = Shader.PropertyToID("_Color");
    int textureID = Shader.PropertyToID("_MainTex");
    int maxVertNum = Shader.PropertyToID("_MaxVertNum");

    /// <summary>
    /// �����ꂽRT���Y��ɂ���
    /// </summary>
    /// <param name="rt"></param>
    public void ClearRT(RenderTexture rt)
    {
        CommandBuffer command = new CommandBuffer();
        command.SetRenderTarget(rt);
        command.ClearRenderTarget(true, true, Color.clear);
        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
    
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paintable"></param>
    /// <param name="worldPosList">�A���̐��E���W�̓_</param>
    /// <param name="color">�`�������̐F</param>
    public void Paint(Paintable paintable, Vector3[] worldPosList, Color? color = null)
    {
        RenderTexture mask = paintable.GetMask();
        RenderTexture copy = paintable.GetCopy();
        Renderer rend = paintable.GetRenderer();
        //shader��4�����̔z�񂵂��󂯂��Ȃ��̂ŁA�ꉞ�]��
        Vector4[] posList = new Vector4[100];
        for (int i = 0; i < worldPosList.Length; i++)
        {
            posList[i] = worldPosList[i];
        }
        //shader�ϐ��ݒu
        paintMaterial.SetInt(maxVertNum, worldPosList.Length);
        paintMaterial.SetVectorArray("_worldPosList", posList);
        paintMaterial.SetColor(colorID, color ?? Color.red);
        paintMaterial.SetTexture(textureID, copy);

        //render�̖ڕW��mask�ɐݒ肷��
        command.SetRenderTarget(mask);
        //render�J�n
        command.DrawRenderer(rend, paintMaterial, 0);

        //�ڕW��copy�ɐݒ�
        command.SetRenderTarget(copy);
        //mask�̕`���copy�ɉ�����
        command.Blit(mask, copy);

        //render�̖��߂𗬂ꂳ����
        Graphics.ExecuteCommandBuffer(command);
        //���ߑ���N���A
        command.Clear();
    }
    /// <summary>
    /// debug�p�֐�
    /// </summary>
    /// <param name="paintable"></param>
    public void InitUVMask(Paintable paintable)
    {
        RenderTexture mask = paintable.GetMask();
        RenderTexture copy = paintable.GetCopy();
        Renderer rend = paintable.GetRenderer();

        command.SetRenderTarget(mask);
        command.SetRenderTarget(copy);
        command.DrawRenderer(rend, paintMaterial, 0);

        Graphics.ExecuteCommandBuffer(command);
        command.Clear();
    }
    protected override void Awake()
    {
        base.Awake();
        paintMaterial = new Material(texturePaint);
        command = new CommandBuffer();
        command.name = "CommmandBuffer - " + gameObject.name;
    }
}