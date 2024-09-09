// Original: https://github.com/Looking-Glass/JoyconLib
// MIT License 2018 Looking Glass
// https://choosealicense.com/licenses/mit/

// This file is modified by Sakura(さくら) / tbbsakura

/*
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


using System.Collections.Generic;
using UnityEngine;

public class JoyconDemo : MonoBehaviour {
	
	private List<Joycon> joycons;

    // Values made available via Unity
    public float[] stick;
    public UnityEngine.Vector3 gyro;
    public UnityEngine.Vector3 accel;
    public int jc_ind = 0;
    public UnityEngine.Quaternion orientation;

	public UnityEngine.Vector3 _RotationOffset;

	public bool IsLeft => joycons [jc_ind].isLeft;

    const int CALIB_KEY_PATTERN = 10;
    int _calibKeyIndex = 0;
    public int CalibKeyIndex {
        get => _calibKeyIndex;
        set => _calibKeyIndex = (value >= CALIB_KEY_PATTERN || value < 0 ) ? 0 : value;
    }
    public int [,] calibKeyPair = new int[CALIB_KEY_PATTERN,2] {
        { (int)Joycon.Button.SHOULDER_2 , (int)Joycon.Button.SHOULDER_2 }, // ZL/ZR
        { (int)Joycon.Button.SHOULDER_1 , (int)Joycon.Button.SHOULDER_1 }, // L/R
        { (int)Joycon.Button.DPAD_RIGHT , (int)Joycon.Button.DPAD_RIGHT }, // RIGHT/A
        { (int)Joycon.Button.DPAD_DOWN ,  (int)Joycon.Button.DPAD_DOWN }, // DOWN/B
        { (int)Joycon.Button.DPAD_LEFT ,  (int)Joycon.Button.DPAD_LEFT }, // LEFT/Y
        { (int)Joycon.Button.DPAD_UP , (int)Joycon.Button.DPAD_UP }, // UP/X 
        { (int)Joycon.Button.SL , (int)Joycon.Button.SL },
        { (int)Joycon.Button.SR , (int)Joycon.Button.SR },
        { (int)Joycon.Button.MINUS , (int)Joycon.Button.PLUS }, // +/-
        { (int)Joycon.Button.CAPTURE ,  (int)Joycon.Button.HOME }, // HOME/CAPTURE
    };

    public int GetCalibKey(bool isLeft)
    {
        return calibKeyPair[ CalibKeyIndex, isLeft ? 0:1];
    }

	public void Attach() {
		if (joycons.Count > jc_ind)
        {
			Joycon j = joycons [jc_ind];
			j?.Attach();
		}
	}

	public void Detach() {
		if (joycons.Count > jc_ind)
        {
			Joycon j = joycons [jc_ind];
			j?.Detach();
		}
	}

    void Start ()
    {
        gyro = new UnityEngine.Vector3(0, 0, 0);
        accel = new UnityEngine.Vector3(0, 0, 0);
        // get the public Joycon array attached to the JoyconManager in scene
        joycons = JoyconManager.Instance.j;
		if (joycons.Count < jc_ind+1){
			Destroy(gameObject);
		}
	}

    // Update is called once per frame
    void Update () {
		// make sure the Joycon only gets checked if attached
		if (joycons.Count > jc_ind)
        {
			Joycon j = joycons [jc_ind];

			if ( j == null ) return;
			if ( j.isLeft ) 
				transform.position = new UnityEngine.Vector3( 0, 0, 0 );
			else 
				transform.position = new UnityEngine.Vector3( 3, 0, 0 );;

			// GetButtonDown checks if a button has been pressed (not held)
			Joycon.Button calibKey = (Joycon.Button)GetCalibKey(j.isLeft);
            if (j.GetButtonDown(calibKey) )
            {
				Debug.Log ($"CalibKey {calibKey.ToString()} pressed");
				// GetStick returns a 2-element vector with x/y joystick components
				//Debug.Log(string.Format("Stick x: {0:N} Stick y: {1:N}",j.GetStick()[0],j.GetStick()[1]));
            
				// Joycon has no magnetometer, so it cannot accurately determine  its yaw value. Joycon.Recenter allows the user to reset the yaw value.
				j.Recenter ();
			}

#if JOYCONLIB_SAMPLE_CODE_AVAILABLE
			// GetButtonDown checks if a button has been released
			if (j.GetButtonUp (Joycon.Button.SHOULDER_2))
			{
				Debug.Log ("Shoulder button 2 released");
			}
			// GetButtonDown checks if a button is currently down (pressed or held)
			if (j.GetButton (Joycon.Button.SHOULDER_2))
			{
				Debug.Log ("Shoulder button 2 held");
			}

			if (j.GetButtonDown (Joycon.Button.DPAD_DOWN)) {
				Debug.Log ("Rumble");

				// Rumble for 200 milliseconds, with low frequency rumble at 160 Hz and high frequency rumble at 320 Hz. For more information check:
				// https://github.com/dekuNukem/Nintendo_Switch_Reverse_Engineering/blob/master/rumble_data_table.md

				j.SetRumble (160, 320, 0.6f, 200);

				// The last argument (time) in SetRumble is optional. Call it with three arguments to turn it on without telling it when to turn off.
                // (Useful for dynamically changing rumble values.)
				// Then call SetRumble(0,0,0) when you want to turn it off.
			}
#endif

            stick = j.GetStick();

            // Gyro values: x, y, z axis values (in radians per second)
            gyro = j.GetGyro();

            // Accel values:  x, y, z axis values (in Gs)
            accel = j.GetAccel();

            orientation = UnityEngine.Quaternion.Euler(_RotationOffset) * j.GetVector();

			if (j.GetButton(calibKey)){
				gameObject.GetComponent<Renderer>().material.color = Color.yellow;
			} else{
				gameObject.GetComponent<Renderer>().material.color = j.isLeft ? Color.red : Color.blue;
			}
            gameObject.transform.rotation = orientation;
        }
    }
}