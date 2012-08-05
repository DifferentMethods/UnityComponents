using UnityEngine;
using System.Collections;

public class IPCamControl : MonoBehaviour
{
	public string url;
	public string username;
	public string password;
	public Texture2D texture;
	public bool streamOnStart = true;
	
	bool streaming = false;
	
	void Start() {
		if(streamOnStart)
			StartStream();	
	}
	
	
	public void Snapshot ()
	{
		var request = new HTTP.Request ("GET", URL ("/snapshot.cgi"));
		request.Send ((obj) => {
			texture.LoadImage (obj.Bytes);
		});
	}
	
	IEnumerator StreamSnapshots() {
		while(streaming) {
			var request = new HTTP.Request ("GET", URL ("/snapshot.cgi"));
			request.Send ();
			while(!request.isDone) yield return null;
			if(request.response.status == 200) {
				texture.LoadImage (request.response.Bytes);
			}
		}
	}
	
	public void StartStream() {
		if(!streaming) {
			streaming = true;
			StartCoroutine(StreamSnapshots());
		}
	}
	
	public void StopStream() {
		streaming = false;
	}
	
	public void Up ()
	{
		Decoder (0);
	}
	
	public void StopUp ()
	{
		Decoder (1);
	}
	
	public void Down ()
	{
		
		Decoder (2);
	}
	
	public void StopDown ()
	{
		Decoder (3);
	}
	
	public void Left ()
	{
		Decoder (6);
	}
	
	public void StopLeft ()
	{
		Decoder (7);
	}
	
	public void Right ()
	{
		Decoder (4);
	}
	
	public void StopRight ()
	{
		Decoder (5);
	}
	
	public void Center ()
	{
		Decoder (25);
	}
	
	public void VerticalPatrol ()
	{
		Decoder (26);
	}
	
	public void StopVerticalPatrol ()
	{
		Decoder (27);
	}
	
	public void HorizonPatrol ()
	{
		Decoder (28);
	}
	
	public void StopHorizonPatrol ()
	{
		Decoder (29);
	}
	
	public void IOOutputHigh ()
	{
		Decoder (94);
	}
	
	public void IOOutputLow ()
	{
		Decoder (95);
	}
	
	public void SetHighResolution() {
		CameraControl(0, 32);
	}
	
	public void SetLowResolution() {
		CameraControl(0, 8);
	}
	
	public void SetBrightness(int brightness) {
		if(brightness < 0 || brightness > 255) throw new System.ArgumentException("Brightness must be between 0 - 255");
		CameraControl(1, brightness);	
	}
	
	public void SetContrast(int contrast) {
		if(contrast < 0 || contrast > 6) throw new System.ArgumentException("Contrast must be between 0 - 6");
		CameraControl(2, contrast);	
	}
	
	public void Set50hzMode() {
		CameraControl(3, 0);
	}
	
	public void Set60HzMode() {
		CameraControl(3, 1);
	}
	
	public void SetOutdoorMode() {
		CameraControl(3, 2);	
	}
	
	public void SetFlipAndMirror(int e) {
		if(e < 0 || e > 3) throw new System.ArgumentException("e must be 0,1,2 or 3.");
		CameraControl(5, e);	
	}
	
	public void Reboot() {
		var request = new HTTP.Request("GET", URL ("reboot.cgi"));
		request.Send();
	}

	
	void Decoder (int command)
	{
		var u = URL ("/decoder_control.cgi") + "&command=" + command.ToString ();
		Debug.Log(u);
		var request = new HTTP.Request ("GET", u);
		request.Send ((HTTP.Response obj) => {
			Debug.Log (obj.status);	
		});
	}
	
	void CameraControl(int param, int value) {
		var u = URL ("/camera_control.cgi") + string.Format("&param={0}&value={1}", param, value);
		Debug.Log(u);
		var request = new HTTP.Request ("GET", u);
		request.Send ((HTTP.Response obj) => {
			Debug.Log (obj.status);	
		});
	}
	
	string URL (string path)
	{
		return url + path + "?user=" + username + "&pwd=" + password;
	}
}
