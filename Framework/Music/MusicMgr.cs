using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
/// <summary>
/// 音乐音效管理器 用到了之前写的缓存池来管理音效 避免频繁的GC带来的性能问题
/// </summary>
public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource backMusic=null;//背景音乐播放组件

    private float backMusicValue = 0.5f;



    //管理正在播放的音效
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
    /// 播放音效
    /// </summary>
    /// <param name="name">文件名</param>
    /// <param name="isLoop">是否循环</param>
    /// <param name="isSync">是否同步加载</param>
    /// <param name="callBack">加载函数后的回调函数</param>
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
                soundList.Add(source);//记录，方便管理 不存在的时候才加入
                //由于从缓存池取出对象 所以要判断 不要重复添加
            }
            callBack?.Invoke(source);//传递给外部使用
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
    /// 播放或暂停所有音效
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
    /// 清空音效相关记录 过场景时在清空缓存池之前调用!!!!!
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
