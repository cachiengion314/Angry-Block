﻿using UnityEngine;

public class TestingIssue : MonoBehaviour {

	LTDescr lt,ff;
	int id,fid;

	void Start () {
		LeanTween.init();
		
		lt = LeanTween.move(gameObject,100*Vector3.one,2);
		id = lt.id;
		LeanTween.pause(id);

		ff = LeanTween.move(gameObject,Vector3.zero,2);
		fid = ff.id;
		LeanTween.pause(fid);
	}
}
