using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class SFXPlayer : MonoBehaviour
{
    public void PlaySFX(string sfx)
    {
        Addressables.LoadAssetAsync<AudioClip>($"sfx/{sfx}").Completed += (handle) => {
            if (handle.Status == AsyncOperationStatus.Succeeded)
            {
                GameObject tempAudioSource = new GameObject("TempAudio");
                AudioSource audioSource = tempAudioSource.AddComponent<AudioSource>();
                audioSource.clip = handle.Result;
                audioSource.spatialBlend = 0.0f;
                audioSource.Play();

                StartCoroutine(ScheduleRelease(audioSource, handle));
            }
        };
    }

    private System.Collections.IEnumerator ScheduleRelease(AudioSource source, AsyncOperationHandle handle)
    {
        yield return new WaitForSeconds(source.clip.length);
        GameObject.Destroy(source.gameObject);
        Addressables.Release(handle);
    }
}
