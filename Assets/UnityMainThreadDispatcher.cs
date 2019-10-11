/*
Copyright 2015 Pim de Witte All Rights Reserved.

Licensed under the Apache License, Version 2.0 (the "License");
you may not use this file except in compliance with the License.
You may obtain a copy of the License at

    http://www.apache.org/licenses/LICENSE-2.0

Unless required by applicable law or agreed to in writing, software
distributed under the License is distributed on an "AS IS" BASIS,
WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
See the License for the specific language governing permissions and
limitations under the License.
*/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

/// Author: Pim de Witte (pimdewitte.com) and contributors
/// <summary>
/// A thread-safe class which holds a queue with actions to execute on the next Update() method. It can be used to make calls to the main thread for
/// things such as UI Manipulation in Unity. It was developed for use in combination with the Firebase Unity plugin, which uses separate threads for event handling
/// </summary>
public class UnityMainThreadDispatcher : MonoBehaviour {

	private static readonly Queue<Action> _executionQueue = new Queue<Action>();


    public float eye_face;

    [System.Serializable]
    class FaceBlendShape
    {
        public float browOuterUpRight, mouthLowerDownLeft, eyeLookUpLeft, cheekPuff, eyeWideLeft, mouthUpperUpLeft, mouthPucker, mouthDimpleRight, mouthSmileRight, mouthShrugLower, eyeLookDownLeft, browOuterUpLeft, eyeBlinkLeft, mouthPressLeft, tongueOut, mouthFrownRight, jawLeft, mouthRight, cheekSquintRight, jawRight, mouthClose, mouthRollLower, eyeSquintLeft, eyeLookUpRight, mouthStretchRight, mouthPressRight, eyeBlinkRight, eyeSquintRight, eyeLookInRight, mouthLeft, mouthRollUpper, noseSneerLeft, eyeLookDownRight, browDownRight, browDownLeft, mouthStretchLeft, mouthDimpleLeft, mouthLowerDownRight, jawOpen, browInnerUp, mouthFunnel, mouthFrownLeft, eyeWideRight, jawForward, eyeLookInLeft, mouthShrugUpper, eyeLookOutLeft, eyeLookOutRight, mouthSmileLeft, cheekSquintLeft, mouthUpperUpRight, noseSneerRight;

        public string[] faceList = {"browOuterUpRight","mouthLowerDownLeft","eyeLookUpLeft","cheekPuff","eyeWideLeft","mouthUpperUpLeft","mouthPucker","mouthDimpleRight","mouthSmileRight","mouthShrugLower","eyeLookDownLeft","browOuterUpLeft","eyeBlinkLeft","mouthPressLeft","tongueOut","mouthFrownRight","jawLeft","mouthRight","cheekSquintRight","jawRight","mouthClose","mouthRollLower","eyeSquintLeft","eyeLookUpRight","mouthStretchRight","mouthPressRight","eyeBlinkRight","eyeSquintRight","eyeLookInRight","mouthLeft","mouthRollUpper","noseSneerLeft","eyeLookDownRight","browDownRight","browDownLeft","mouthStretchLeft","mouthDimpleLeft","mouthLowerDownRight","jawOpen","browInnerUp","mouthFunnel","mouthFrownLeft","eyeWideRight","jawForward","eyeLookInLeft","mouthShrugUpper","eyeLookOutLeft","eyeLookOutRight","mouthSmileLeft","cheekSquintLeft","mouthUpperUpRight","noseSneerRight"};

        public  void SetDate(int order ,float data){
            faceList[order] = data;
            cheekPuff = data;
            
        }
	}
    [SerializeField] FaceBlendShape faceBlend;
    public float[] face_list　= new float[65];

    private bool nameListBool = false;

    public string[] name_all=new string[65];

    

    /*
	    public void start(){
        faceblendshape = new FaceBlendShape();
    }
	 */

    public void Update() {
        FaceBlendShape faceblendshape = new FaceBlendShape();
        var net = NetworkMeshAnimator.Instance;
        var nameList = net.blendShapeName;

		for (int i = 0; i < nameList.Count; i++){
			faceblendshape.SetDate(i,net.blendShapeList[i]);
		}
        Debug.Log(faceblendshape.cheekPuff);



        /*
                if(!nameListBool){
                    var net1 = NetworkMeshAnimator.Instance;
                    var nameList = net1.blendShapeName;
                    Debug.Log(nameList);
                    Debug.Log(nameList.Count);
                    for (int i = 0; i < nameList.Count; i++)
                    {

                        Debug.Log(nameList[i]);
                        name_all[i] = nameList[i];

                        if(i>19){
                            nameListBool = true;
                        }

                    }
                    Debug.Log(string.Join(",", name_all));
                }
         */



        //eye_face = net.temp;
        face_list = net.blendShapeList;
        //Debug.Log(net.blendShapeList);
        lock(_executionQueue) {
			while (_executionQueue.Count > 0) {
				_executionQueue.Dequeue().Invoke();
			}
		}
	}

	/// <summary>
	/// Locks the queue and adds the IEnumerator to the queue
	/// </summary>
	/// <param name="action">IEnumerator function that will be executed from the main thread.</param>
	public void Enqueue(IEnumerator action) {
		lock (_executionQueue) {
			_executionQueue.Enqueue (() => {
				StartCoroutine (action);
			});
		}
	}

        /// <summary>
        /// Locks the queue and adds the Action to the queue
	/// </summary>
	/// <param name="action">function that will be executed from the main thread.</param>
	public void Enqueue(Action action)
	{
		Enqueue(ActionWrapper(action));
	}
	IEnumerator ActionWrapper(Action a)
	{
		a();
		yield return null;
	}


	private static UnityMainThreadDispatcher _instance = null;

	public static bool Exists() {
		return _instance != null;
	}

	public static UnityMainThreadDispatcher Instance() {
		if (!Exists ()) {
			throw new Exception ("UnityMainThreadDispatcher could not find the UnityMainThreadDispatcher object. Please ensure you have added the MainThreadExecutor Prefab to your scene.");
		}
		return _instance;
	}


	void Awake() {
		if (_instance == null) {
			_instance = this;
			DontDestroyOnLoad(this.gameObject);
		}
	}

	void OnDestroy() {
			_instance = null;
	}


}
