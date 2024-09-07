/*
MIT License 2024 Sakura(さくら) / tbbsakura

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.*/

using UnityEngine;
using UnityEngine.UI;
using uOSC;
using System.Net;
using System;


public class JoyToVRCUI : MonoBehaviour
{
    public uOscClient _OscClient;
    public GameObject JCM;
    public JoyconDemo Con1;
    public JoyconDemo Con2;

    public InputField _IPADDR;
    public InputField _port;

    // Start is called before the first frame update
    void Start()
    {
        SetIPPortUI();
    }

    void SetIPPortUI()
    {
        _IPADDR.text = _OscClient.address;
        _port.text = _OscClient.port.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if ( _OscClient.isRunning ) {
            this.SendOSC();
        }
    }

    // 回転してくれないときに再接続できないか試したが現状無意味
    public void ButtonReL()
    {
        if (Con1.IsLeft) { Con1.Detach(); Con1.Attach(); }
        if (Con2.IsLeft) { Con2.Detach(); Con2.Attach(); }
    }
    public void ButtonReR()
    {
        if (!Con1.IsLeft) { Con1.Detach(); Con1.Attach(); }
        if (!Con2.IsLeft) { Con2.Detach(); Con2.Attach(); }
    }

    public void OnToggleJoyConStart( bool isOn ) {
        Debug.Log($"OnToggleJoyConStart: isOn = {isOn}");
        if ( isOn ) {
            JCM.SetActive(true);
            Con1.gameObject.SetActive(true);
            Con2.gameObject.SetActive(true);
        }
        else {
            Con1.gameObject.SetActive(false);
            Con2.gameObject.SetActive(false);
            JCM.SetActive(false);
        }
    }

    bool IsValidIpAddr( string ipString ) {
        IPAddress address;
        return (IPAddress.TryParse(ipString, out address));
    }

    int GetValidPortFromStr( string portstr )
    {
        int port;
        if (!int.TryParse(portstr, out port)) {
            return -1;
        }
        if (port <= 0 || port > 65535 ) {
            return -1;
        }
        return port;
    }
    
    public void OnToggleSending(bool isOn)
    {
        Debug.Log($"OnToggleSending: isOn = {isOn}");
        int port = GetValidPortFromStr(_port.text);
        if ( IsValidIpAddr(_IPADDR.text) && port != -1 ) {
            _OscClient.address = _IPADDR.text;
            _OscClient.port = port;
            _OscClient.gameObject.SetActive(isOn);
            SetIPPortUI();
        }
        else {
            // エラーメッセージを出すところだが、デフォルトを変える必要がないので省略している（よくない）
        }
    }


/*
        // HUMANOID BONES of arms and hands
        "/avatar/parameters/TMARelay_UpperArmL_DownUp", //39 : -60 to 100
        "/avatar/parameters/TMARelay_UpperArmL_FrontBack", //40 : -100 to 100
        "/avatar/parameters/TMARelay_UpperArmL_Twist", //41 : -90 to 90
        "/avatar/parameters/TMARelay_LowerArmL_Stretch", //42 : -80 to 80
        "/avatar/parameters/TMARelay_LowerArmL_Twist", //43 : -90 to 80
        "/avatar/parameters/TMARelay_HandL_DownUp", //44 (Ignore) -80 to 80
        "/avatar/parameters/TMARelay_HandL_InOut", //45 (Ignore) -40 to 40

        "/avatar/parameters/TMARelay_UpperArmR_DownUp", //48 
        "/avatar/parameters/TMARelay_UpperArmR_FrontBack", //49
        "/avatar/parameters/TMARelay_UpperArmR_Twist", //50 
        "/avatar/parameters/TMARelay_LowerArmR_Stretch", //51
        "/avatar/parameters/TMARelay_LowerArmR_Twist", //52 
        "/avatar/parameters/TMARelay_HandR_DownUp", //53 (Ignore)
        "/avatar/parameters/TMARelay_HandR_InOut", //54 (Ignore)
*/

    public string oscad1L = "TMARelay_LowerArmL_Stretch";
    public string oscad2L = "TMARelay_LowerArmL_Twist";
    public string oscad3L = "TMARelay_UpperArmL_Twist";
    public string oscad1R = "TMARelay_LowerArmR_Stretch";
    public string oscad2R = "TMARelay_LowerArmR_Twist";
    public string oscad3R = "TMARelay_UpperArmR_DownUp";

    public static float Map2(float value, float fromSource, float toSource, float fromTarget, float toTarget)
    {
        return (value - fromSource) / (toSource - fromSource) * (toTarget - fromTarget) + fromTarget;
    }

    float ClampMap01(float value, float start1, float stop1)
    {
        float valC = Math.Clamp(value,start1, stop1);
        return Map2( value, start1, stop1, 0f, 2.0f)-1;
    }

    // 今のところ3つとも同じだが、変えたい場合に備えて分けてある
    // -90(↓)～0度(正面)~90度 等であるのを、マイナス部分が180-360で読み取られるため、180度回す。
    float PitchToMuscle( float rot ) {
        float adjX = (rot > 180) ? rot - 180 : rot + 180; 
        return ClampMap01(adjX, 90,270);
    }

    float RollToMuscle( float rot ) {
        float adjY = (rot > 180) ? rot - 180 : rot + 180; 
        return ClampMap01(adjY, 90,270);
    }

    float YawToMuscle( float rot ) {
        float adjZ = (rot > 180) ? rot - 180 : rot + 180; 
        return ClampMap01(adjZ, 90,270);
    }

    void SendOSC()
    {
        UnityEngine.Vector3 rot1 = Con1.transform.rotation.eulerAngles;
        UnityEngine.Vector3 rot2 = Con2.transform.rotation.eulerAngles;
        UnityEngine.Vector3 rotL = Con1.IsLeft ? rot1 : rot2;
        UnityEngine.Vector3 rotR = Con1.IsLeft ? rot2 : rot1;
        // TMARelay_LowerArmL_Stretch 腕の正面上げ下げにする
        _OscClient.Send("/avatar/parameters/" + oscad1L, PitchToMuscle(rotL.x) );
        _OscClient.Send("/avatar/parameters/" + oscad1R, PitchToMuscle(rotR.x) );

        // rot.y を手を振るようなArmの回転に(左右で反転)
        _OscClient.Send("/avatar/parameters/" + oscad2L, RollToMuscle(rotL.y)*-1);
        _OscClient.Send("/avatar/parameters/" + oscad2R, RollToMuscle(rotR.y));

        // rot.z を手首の回転にする : TODO こちらも反転すべきか確認
        _OscClient.Send("/avatar/parameters/" + oscad3L, YawToMuscle(rotL.z));
        _OscClient.Send("/avatar/parameters/" + oscad3R, YawToMuscle(rotR.z));

    }
}
