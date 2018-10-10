using System;
#if UNITY_UWP || WINDOWS_UWP
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
#endif
using UnityEngine;

namespace HoloToolkitExtensions.RemoteAssets
{
    public class UWPDynamicTextureDownloader : MonoBehaviour
    {
        protected MainThreadExecuter _mainThreadExecuter;
        
        public string ImageUrl;

        private string _previousImageUrl = null;

        private Vector3 _originalScale;
#if UNITY_UWP || WINDOWS_UWP
        public HttpClient HttpClientExecuter { get; set; }
#endif

        void Start()
        {
            _mainThreadExecuter = GetComponent<MainThreadExecuter>();
            _originalScale = transform.localScale;
        }

        void Update()
        {
            CheckLoadImage();
            OnUpdate();
        }

        private void CheckLoadImage()
        {
            // No image requested
            if (string.IsNullOrEmpty(ImageUrl))
            {
                return;
            }

            // New image set - reset status vars and start loading new image
            if (_previousImageUrl != ImageUrl)
            {
                _previousImageUrl = ImageUrl;

                OnStartLoad();
            }
        }


        protected virtual void OnStartLoad()
        {
           // Debug.Log("Start Loading" + ImageUrl);
#if UNITY_UWP || WINDOWS_UWP
            LeanTween.cancel(gameObject);
            LeanTween.alpha( gameObject,0, 0.5f).setOnComplete(() => ReadImageDataFromWeb());
#endif
        }

#if UNITY_UWP || WINDOWS_UWP
        private async Task ReadImageDataFromWeb()
        {
            //Debug.Log("Start downloading tile " + ImageUrl );
            var request = new HttpRequestMessage(HttpMethod.Get, new Uri(ImageUrl));
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("Chrome", "61.0.3163.100"));
            using (var httpResponse = await HttpClientExecuter.SendAsync(request))
            {
                if (httpResponse.IsSuccessStatusCode)
                {
                    var data = await httpResponse.Content.ReadAsByteArrayAsync();
                  //  Debug.Log("downloaded " + ImageUrl);

                    ProcessWebResponse(data);
                }
            }
        }
#endif

        private void ProcessWebResponse(byte[] data)
        {
            //Debug.Log("Finished downloading " + ImageUrl + ":" + data.Length);
            _mainThreadExecuter.Add(() =>
            {
                var downloadedImage = new Texture2D(0, 0);
                downloadedImage.LoadImage(data);
                var material = GetComponent<Renderer>().material;
                Destroy(material.mainTexture);
                material.mainTexture = downloadedImage;
                OnEndLoad();
            });
        }

        protected virtual void OnEndLoad()
        {

        }

        protected virtual void OnUpdate()
        {

        }
    }
}
