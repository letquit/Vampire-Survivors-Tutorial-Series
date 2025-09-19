using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

public class AssetLoader : MonoBehaviour
{
    public AssetLabelReference assetLabel;
    public Image[] menuImgs;
    
    private AsyncOperationHandle groupHandle;
    
    public void LoadAssetGroup()
    {
        if (groupHandle.IsValid())
        {
            return;
        }

        Addressables.LoadAssetsAsync<Sprite>(assetLabel, null).Completed += handle =>
        {
            for (int n = 0; n < handle.Result.Count; n++)
            {
                Debug.Log(handle.Result[n].name);
                if (handle.Result[n].name == "backgroundtest1")
                {
                    menuImgs[0].sprite = handle.Result[n];
                }
                else if (handle.Result[n].name == "backgroundtest2")
                {
                    menuImgs[1].sprite = handle.Result[n];
                }
                else if (handle.Result[n].name == "backgroundtest3")
                {
                    menuImgs[2].sprite = handle.Result[n];
                }
                else if (handle.Result[n].name == "backgroundtest4")
                {
                    menuImgs[3].sprite = handle.Result[n];
                }
            }
            groupHandle = handle;
        };
    }
    
    public void ReleaseAssetGroup()
    {
        if (groupHandle.IsValid())
        {
            for (int n = 0; n < menuImgs.Length; n++)
            {
                menuImgs[n].sprite = null;
            }
            
            Addressables.Release(groupHandle);
        }
    }
}
