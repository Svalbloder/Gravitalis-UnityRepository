using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    public GameObject mainMenu;
    public GameObject[] panels;
    public AudioMixer AudioMixer;
    public int cameraSize;
    public Camera Camera => Camera.main;
    public Collider2D HitCollider { get; private set; }

    public void Awake()
    {
        if(mainMenu!=null)
        mainMenu.gameObject.SetActive(true);
        if (panels != null)
        {
            foreach (GameObject panel in panels)
            {
                panel.SetActive(false);
            }
        }      
        if (instance != null)
            Destroy(this);
        instance = this;

        Application.runInBackground = true;
#if !UNITY_EDITOR && UNITY_STANDALONE_WIN
        WindowsAPI.InitWindow();
#endif

    }
    private void Update()
    {
        HitCollider = Physics2D.OverlapPoint(Camera.ScreenToWorldPoint(Input.mousePosition));
        WindowsAPI.SetClickThrough(HitCollider);
    }
    public void QuitGame()
    {
        // �ڱ༭�����޷��˳�Ӧ�ó��򣬵��ڹ�������Ϸ�л���Ч
        Application.Quit();

        // ������ڱ༭���н��в���
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
    public void PauseGame()
    {
        Time.timeScale = 0f; // ��ͣʱ��
    }
    // ������Ϸ�ķ���
    public void ResumeGame()
    {
        Time.timeScale = 1f; // �ָ�ʱ��
    }
    public void SetBGMVolume(float value)
    {
        if (AudioMixer == null)
            return;
        AudioMixer.SetFloat("BGM", value);
        if (value <= -20f)
        {
            AudioMixer.SetFloat("BGM", -80);
        }
    }
    public void SetSFXVolume(float value)
    {
        if (AudioMixer == null)
            return;
        AudioMixer.SetFloat("SFX", value);
        if (value <= -20f)
        {
            AudioMixer.SetFloat("SFX", -80);
        }
    }
    public void ChangeSize(bool value)
    {
        if (value)
        {
            cameraSize += 10;
            Camera.main.orthographicSize = cameraSize;
        }
        else
        {
            cameraSize -= 10;
            Camera.main.orthographicSize = cameraSize;
        }

    }
}
