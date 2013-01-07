using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//TODO: Should this be called RoarActionsWidget?

public class RoarLoginWidget : RoarUIWidget
{
	
	protected Roar.Components.IActions actions;
	protected bool isFetching=false;
	
	public bool fetchPropertiesOnLogin = true;
	public bool saveUsername = true;
	public bool savePassword = false;
	
	private static string KEY_USERNAME = "roar-username";
	private static string KEY_PASSWORD = "roar-password";
	
	private string status = "Supply a username and password to log in or to register a new account.";
	private bool isError;
	private string username = string.Empty;
	private string password = string.Empty;
	public int loginBoxSpacing = 50;
	public int leftOffset = 250;
	public int topOffset = 50;
	public int passwordBoxSpacing = 50;
	public float labelWidth = 90;
	public float fieldHeight = 30;
	public float fieldWidth = 140;
	public float buttonWidth = 120;
	public float buttonHeight = 40;
	public float buttonSpacing = 40;
	private bool networkActionInProgress;
	public string buttonStyle="LoginButton";
	public string boxStyle="LoginField";
	public string loginLabelStyle = "LoginLabel";
	public string statusStyle = "LoginStatus";
	
	
	protected override void OnEnable ()
	{
		if (saveUsername)
		{
			username = PlayerPrefs.GetString(KEY_USERNAME, string.Empty);
		}
		if (savePassword)
		{
			password = PlayerPrefs.GetString(KEY_PASSWORD, string.Empty);
		}
		
		
	}
	
		
	protected override void DrawGUI(int windowId)
	{
		if(networkActionInProgress)
		{
			GUI.Label(new Rect(0,0,ContentWidth,ContentHeight), "Logging In", "StatusNormal");
			ScrollViewContentHeight = contentBounds.height;
		}
		else
		{
		
			Rect currentRect = new Rect(leftOffset, topOffset, contentBounds.width, fieldHeight);
		GUI.Label(currentRect, status, statusStyle);
			currentRect.y += 100;
		currentRect.width = labelWidth;
		GUI.Label(currentRect, "Username", loginLabelStyle);
			currentRect.x += labelWidth;
			currentRect.width = fieldWidth;
			
		username = GUI.TextField(currentRect, username, boxStyle);
			currentRect.x = leftOffset;
			
		currentRect.y += fieldHeight + loginBoxSpacing;
			
			currentRect.width = labelWidth;
		GUI.Label(currentRect, "Password", loginLabelStyle);
			currentRect.x += labelWidth;
			currentRect.width = fieldWidth;
			
		password = GUI.PasswordField(currentRect, password, '*', boxStyle);
			currentRect.x = leftOffset;
			currentRect.y+= fieldHeight + passwordBoxSpacing;
			
			currentRect.width = buttonWidth;
			currentRect.height = buttonHeight;
			
		GUI.enabled = username.Length > 0 && password.Length > 0 && !networkActionInProgress;
		
		if (GUI.Button(currentRect, "Log In", buttonStyle))
		{

			status = "Logging in...";
			networkActionInProgress = true;			
			if (saveUsername)
			{
				PlayerPrefs.SetString(KEY_USERNAME, username);
			}
			if (savePassword)
			{
				PlayerPrefs.SetString(KEY_PASSWORD, password);
			}
			if (Debug.isDebugBuild)
				Debug.Log(string.Format("[Debug] Logging in as [{0}] with password [{1}].", username, password));
			roar.Login(username, password, OnRoarLoginComplete);
		}
			currentRect.y+= buttonSpacing + buttonHeight;
		if (GUI.Button(currentRect, "Create Account", buttonStyle))
		{

			status = "Creating new player account...";
			networkActionInProgress = true;
			roar.Create(username, password, OnRoarAccountCreateComplete);
		}
		
		
		}
		
	}
	
	public void Fetch()
	{
		isFetching = true;
		actions.Fetch(OnRoarFetchActionsComplete);
	}
	
	void OnRoarFetchActionsComplete( Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.Task> > data )
	{
		isFetching = false;
	}
	
	void OnRoarLoginComplete(Roar.CallbackInfo<Roar.WebObjects.User.LoginResponse> info)
	{
		if (Debug.isDebugBuild)
		{
			Debug.Log(string.Format("OnRoarLoginComplete ({0}): {1}", info.code, info.msg));
			if( info.data!= null)
			{
			Debug.Log( string.Format("OnRoarLoginComplete got auth_token {0}", info.data.auth_token ) );
			Debug.Log( string.Format("OnRoarLoginComplete got player_id {0}",  info.data.player_id ) );
			}
			else
			{
				Debug.Log("OnRoarLogingComplete got null data");
			}
		}
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			isError = false;
			this.enabled = false;
			// fetch the player's properties after successful login
			if (fetchPropertiesOnLogin)
			{
				DefaultRoar.Instance.Properties.Fetch(OnRoarPropertiesFetched);
			}
			else
			{
				
				networkActionInProgress = false;
			}
			
			break;
		case 3: // Invalid name or password
		default:
			isError = true;
			status = info.msg;
			networkActionInProgress = false;
			break;
		}
	}

	void OnRoarAccountCreateComplete(Roar.CallbackInfo<Roar.WebObjects.User.CreateResponse> info)
	{
		if (Debug.isDebugBuild)
			Debug.Log(string.Format("OnRoarAccountCreateComplete ({0}): {1}", info.code, info.msg));
		switch (info.code)
		{
		case IWebAPI.OK: // (success)
			status = "Account successfully created. You can now log in.";
			break;
		case 3:
		default:
			isError = true;
			status = info.msg;
			break;
		}
		networkActionInProgress = false;
	}
	
	void OnRoarPropertiesFetched(Roar.CallbackInfo< IDictionary<string,Roar.DomainObjects.PlayerAttribute> > info)
	{
		networkActionInProgress = false;
		
		//if (OnFullyLoggedIn != null) OnFullyLoggedIn();
	}

}