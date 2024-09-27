using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// ������Ч������ �õ���֮ǰд�Ļ������������Ч ����Ƶ����GC��������������
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource backMusic=null;//�������ֲ������

    private float backMusicValue = 0.5f;



    //�������ڲ��ŵ���Ч
    private List<AudioSource>soundList=new List<AudioSource>();

    private float soundValue = 0.5f;

    private bool soundIsPlaying=true;
    private MusicMgr()
    {
        MonoMgr.Instance.AddUpdateListener(Update);
    }
    private void Update()
    {
        if (!soundIsPlaying) return;
        for(int i=soundList.Count-1; i>=0; i--)
        {
            if(!soundList[i].isPlaying)
            {
                soundList[i].clip = null;
                PoolMgr.Instance.PushObj(soundList[i].gameObject);
                soundList.RemoveAt(i);
            }
        }
    }
    public void PlayBackMusic(string name)
    {
        if(backMusic == null)
        {
            GameObject obj=new GameObject();
            obj.name = "BackMusic";
            GameObject.DontDestroyOnLoad(obj);
            backMusic=obj.AddComponent<AudioSource>();
        }
        ABResMgr.Instance.LoadResAsync<AudioClip>("music", name, (clip) =>
        {
            backMusic.clip = clip;
            backMusic.loop = true;
            backMusic.volume = backMusicValue;
            backMusic.Play();
        });
    }
    public void StopBackMusic()
    {
        if (backMusic == null) return;
        backMusic.Stop();
    }
    public void PauseBackMusic()
    {
        if (backMusic == null) return;
        backMusic.Pause();
    }
    public void ChangeBackMusicVolume(float v)
    {

        backMusicValue = v;
        if (backMusic == null) return;
        
        backMusic.volume = backMusicValue;
    }
    /// <summary>
    /// ������Ч
    /// </summary>
    /// <param name="name">�ļ���</param>
    /// <param name="isLoop">�Ƿ�ѭ��</param>
    /// <param name="isSync">�Ƿ�ͬ������</param>
    /// <param name="callBack">���غ�����Ļص�����</param>
    public void PlaySound(string name,bool isLoop=false,bool isSync=false,UnityAction<AudioSource>callBack=null)
    {
        ABResMgr.Instance.LoadResAsync<AudioClip>("sound", name, (clip) =>
        {
            AudioSource source=PoolMgr.Instance.GetObj("Sound/soundObj").GetComponent<AudioSource>();
            source.Stop();

            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundValue;
            source.Play();

            if (!soundList.Contains(source))
            {
                soundList.Add(source);//��¼��������� �����ڵ�ʱ��ż���
                //���ڴӻ����ȡ������ ����Ҫ�ж� ��Ҫ�ظ����
            }
            callBack?.Invoke(source);//���ݸ��ⲿʹ��
        },isSync);
    }
    public void StopSound(AudioSource source)
    {
        if(soundList.Contains(source))
        {
            source.Stop();
            soundList.Remove(source);
            source.clip=null;
            PoolMgr.Instance.PushObj(source.gameObject);
        }
    }
    public void ChangeSoundVolume(float v)
    {
        soundValue = v;
        for (int i = 0; i < soundList.Count; i++)
        {
            soundList[i].volume = soundValue;
        }
    }
    /// <summary>
    /// ���Ż���ͣ������Ч
    /// </summary>
    /// <param name="isPlay"></param>
    public void PlayOrPauseSound(bool isPlay)
    {
        if (isPlay)
        {
            soundIsPlaying = true;
            for(int i = 0;i < soundList.Count;i++)
            {
                soundList[i].Play();
            }
        }
        else
        {
            soundIsPlaying = false;
            for(int i = 0;i<soundList.Count; i++)
            {
                soundList[i].Pause();
            }
        }
    }
    /// <summary>
    /// �����Ч��ؼ�¼ ������ʱ����ջ����֮ǰ����!!!!!
    /// </summary>
    public void ClearSound()
    {
        for(int i = 0; i < soundList.Count; i++)
        {
            soundList[i].Stop();
            soundList[i].clip = null;
            PoolMgr.Instance.PushObj(soundList[i].gameObject);
        }
        soundList.Clear();
    }
}
