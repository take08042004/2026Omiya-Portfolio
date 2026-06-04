//using System.Numerics;
//Vector2がUnityEngineにあるためCO、System.Numericsの方は使用しないようにする
using UnityEngine;
using Windows.Kinect; // Kinectの機能を使うために必要な名前空間

// クラスの名前を先に定義
public class HandInput : MonoBehaviour
{
    //出力データ
    private bool isFist;
    private bool isOpen;//追加
    private Vector2 handPosition; //座標値

    //Kinectを使用する際の必要な変数
    private KinectSensor sensor;
    private BodyFrameReader reader;
    private Body[] bodies;
    private CoordinateMapper coordinateMapper; // Kinectの座標変換用のオブジェクト

    [Header("調整")]
    public float smoothFactor = 0.2f;

    void Start()
    {
        sensor = KinectSensor.GetDefault();

        if(sensor != null)
        {   //追加部分
            coordinateMapper = sensor.CoordinateMapper; // Kinectの座標変換用のオブジェクトを取得

            reader = sensor.BodyFrameSource.OpenReader();

            if (!sensor.IsOpen)
            {
                sensor.Open();
            }
        }
        else
        {
            Debug.LogError("KinectSensor is not available.");
        }
    }

    void Update()
    {
        if(reader == null) return;

        var frame = reader.AcquireLatestFrame();

        if(frame == null) return;
        
        if(bodies == null)
        {
            bodies = new Body[sensor.BodyFrameSource.BodyCount];
        }

        frame.GetAndRefreshBodyData(bodies);
        frame.Dispose();

        bool tracked = false;

        foreach(var body in bodies)
        {
            if(body == null) continue;
            if(!body.IsTracked) continue;

            tracked = true;

            //isFist = body.HandRightState == HandState.Closed;
            HandState handstate = body.HandRightState;

            isFist = handstate == HandState.Closed;
            isOpen = handstate == HandState.Open; 

                // Kinectを使用した座標の取得

            //Joint表記は重複するためWindows.Kinect.Jointを明示的に指定
            Windows.Kinect.Joint joint = body.Joints[JointType.HandRight];

            /*Vector3 worldPos = new Vector3
            (
                joint.Position.X,
                joint.Position.Y,
                joint.Position.Z
            );

                //　画面座標への変換
            if(worldPos.z < 0)
            {
                worldPos.z *= -1;
            }

            Vector3 screenPos = Camera.main.WorldToScreenPoint(worldPos);*/

            //置き換え部分
            CameraSpacePoint cameraSpacePoint = joint.Position;

            ColorSpacePoint colorPoint = coordinateMapper.MapCameraPointToColorSpace(cameraSpacePoint);

            float screenX = (colorPoint.X / 1920f) * Screen.width; // Kinectの解像度に基づいて画面座標に変換
            float screenY = Screen.height - ((colorPoint.Y / 1080f) * Screen.height); // Y座標は上下逆になるため反転

            screenX = Mathf.Clamp(screenX, 0, Screen.width);
            screenY = Mathf.Clamp(screenY, 0, Screen.height);

            Vector2 targetPos = new Vector2(screenX, screenY);

            //置き換え終了

                // スムージング
            handPosition = Vector2.Lerp(
                handPosition,
                //new Vector2(screenPos.x, screenPos.y),
                targetPos, //追加
                smoothFactor
            );

            break; // 最初のトラッキングされた体だけを処理する
        }

        if (!tracked)
        {
            isFist = false; // トラッキングされていない場合は「グーじゃない」とする
        }
        
    }


    // あとでここに手の座標やグーパー判定の処理を書く
    // 今はエラーを消すため色々必要なものを仮置き
    public bool IsFist() 
    { 
        return isFist; 
    }

    ///追加
    public bool IsOpen()
    {
        return isOpen;
    }

    public Vector2 GetHandPosition()
    {
        return handPosition;
    }

    void OnApplicationQuit()
    {
        if(reader != null)
        {
            reader.Dispose();
            reader = null;
        }

        if(sensor != null && sensor.IsOpen)
        {
            sensor.Close();
        }
    }

    public bool IsGrabbing()
    {
        return IsFist();
    }
}