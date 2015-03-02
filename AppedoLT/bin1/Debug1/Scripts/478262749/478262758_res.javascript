var BrowserDetect = null;
var strImageCode = null;

/*
// ExtJS login form
var loginFormPanel = null;
Ext.onReady(function(){
	loginFormPanel = new Ext.form.FormPanel({
		standardSubmit: true,
		border: false,
		bodyStyle: "background-color: transparent",
		style: "padding: 8px 8px 8px 8px; ",
		width: 340,
		labelWidth: 100,
		defaults: {width: 200},
		defaultType: 'textfield',
		items: [
			{
				fieldLabel: 'User Name',
				name: 'username',
				id: 'username',
				allowBlank:false,
				cls: 'textbox'
			},{
				fieldLabel: 'Password',
				name: 'password',
				id: 'password',
				inputType:'password',
				allowBlank:false,
				cls: 'textbox'
			},{
				fieldLabel: 'Enterprise',
				name: 'Enterprise',
				id: 'Enterprise',
				allowBlank:false,
				cls: 'textbox',
				editable: false,
				readOnly : true
			}
		],
		buttonAlign: 'center',
		buttons: [
			'','','','','',
			{
				//text: 'Submit',
				iconCls: 'btnSubmit',
				bodyStyle: "border: 0; background-color: transparent",
				handler: loginSession,
				formBind: true,
				keys: new Ext.KeyMap(document, {
					key: Ext.EventObject.ENTER,
					handler: loginSession
				})
			},{
				//text: 'Cancel',
				iconCls: 'btnCancel',
				bodyStyle: "border: 0; background-color: transparent",
				handler: resetForm
			}]
	});
	loginFormPanel.render('login');
	Ext.get("username").focus();
	loginFormPanel.getForm().findField('Enterprise').setValue(getEnterpriseCode());
});

function loginSession() {
	loginFormPanel.getForm().getEl().dom.action = 'loginsession';
	loginFormPanel.getForm().getEl().dom.method = 'POST';
	loginFormPanel.getForm().submit();
}

function resetForm(){
	var str_username = loginFormPanel.getComponent('username');
	str_username.setValue("");
	var str_password = loginFormPanel.getComponent('password');
	str_password.setValue("");
}
*/
function validateForm(){
	var loginFormPanel = document.getElementById("loginFormPanel");
	var userId = loginFormPanel.username.value;
	if( userId.length <=0 ){
		document.getElementById("message").innerHTML = "UserId is empty";
		return false;
	}else{
		var password = loginFormPanel.password.value;
		if( password.length <=0 ){
			document.getElementById("message").innerHTML = "Password is empty";
			return false;
		}
	}
	return true;
}

function loginSession() {
	var bValidataion = validateForm();
	if( bValidataion ){
		document.getElementById("loginFormPanel").action = "./loginsession";
		document.getElementById("loginFormPanel").submit();
	}
}

function resetForm(){
	document.getElementById('username').value='';
	document.getElementById('password').value='';
	document.getElementById("message").value='';
}

function loadXMLDoc()
{
	var xmlhttp;
	if (window.XMLHttpRequest){							// code for IE7+, Firefox, Chrome, Opera, Safari
		xmlhttp=new XMLHttpRequest();
	}else{												// code for IE6, IE5
		xmlhttp=new ActiveXObject("Microsoft.XMLHTTP");
	}
	xmlhttp.onreadystatechange=function()
	{
		if (xmlhttp.readyState==4 && xmlhttp.status==200)
		{
			var jsonData = eval('('+xmlhttp.responseText+')');
			var imageCode = jsonData.data;
			strImageCode = imageCode.strImageCode
		}
	}
	xmlhttp.open("GET","view/getImageCode.jsp?"+(new Date()).getTime(),true);
	xmlhttp.send();
}

function showForgotPassword(){
	document.getElementById("tdSecurityCap").style.display = 'block';
}

function validateFormForForgotPassword(){
	var loginFormPanel = document.getElementById("loginFormPanel");
	var userId = loginFormPanel.username.value;
	var spam = document.getElementById("spam").value;
	
	if( userId.length <=0 ){
		document.getElementById("message").innerHTML = "UserId is empty";
		return false;
	}else if( spam != strImageCode ){
		document.getElementById("message").innerHTML = "Security code did not match";
		return false;
	}
	return true;
}

function forgotPassword(){
	if( !validateFormForForgotPassword() ){
		return false;
	} else {
		document.getElementById("loginFormPanel").action = "./forgotPassword";
		document.getElementById("loginFormPanel").submit();
	}
}

function checkclear(what){
	if(!what._haschanged){
		what.value=''
	};
	what._haschanged=true;
}

function checkBrowser(){
	if( BrowserDetect.browser != 'Explorer' && BrowserDetect.browser != 'Chrome' && !(BrowserDetect.browser == 'Firefox' && BrowserDetect.version >= 3.6) ){
/*		document.getElementById('username').disabled = true;
		document.getElementById('password').disabled = true;
		document.getElementById("message").disabled = true;
		document.getElementById("buttonSumbit").disabled = true;
		document.getElementById("buttonClear").disabled = true;
*/
		document.getElementById("loginFormPanel").innerHTML = "<table align='center' valign='center' width='80%' ><tr><td><font color='red' >Please use Mozilla Firefox version 3.6+ and Internet Explorer 7+ only.</font></td></tr></table>";
		return false;
	}else{
		return true;
	}
}
function detectBrowser(){
	BrowserDetect = {
		init: function () {
			this.browser = this.searchString(this.dataBrowser) || "An unknown browser";
			this.version = this.searchVersion(navigator.userAgent)
				|| this.searchVersion(navigator.appVersion)
				|| "an unknown version";
			this.OS = this.searchString(this.dataOS) || "an unknown OS";
		},
		searchString: function (data) {
			for (var i=0;i<data.length;i++)	{
				var dataString = data[i].string;
				var dataProp = data[i].prop;
				this.versionSearchString = data[i].versionSearch || data[i].identity;
				if (dataString) {
					if (dataString.indexOf(data[i].subString) != -1)
						return data[i].identity;
				}
				else if (dataProp)
					return data[i].identity;
			}
		},
		searchVersion: function (dataString) {
			var index = dataString.indexOf(this.versionSearchString);
			if (index == -1) return;
			return parseFloat(dataString.substring(index+this.versionSearchString.length+1));
		},
		dataBrowser: [
			{
				string: navigator.userAgent,
				subString: "Chrome",
				identity: "Chrome"
			},
			{ 	string: navigator.userAgent,
				subString: "OmniWeb",
				versionSearch: "OmniWeb/",
				identity: "OmniWeb"
			},
			{
				string: navigator.vendor,
				subString: "Apple",
				identity: "Safari",
				versionSearch: "Version"
			},
			{
				prop: window.opera,
				identity: "Opera"
			},
			{
				string: navigator.vendor,
				subString: "iCab",
				identity: "iCab"
			},
			{
				string: navigator.vendor,
				subString: "KDE",
				identity: "Konqueror"
			},
			{
				string: navigator.userAgent,
				subString: "Firefox",
				identity: "Firefox"
			},
			{
				string: navigator.vendor,
				subString: "Camino",
				identity: "Camino"
			},
			{		// for newer Netscapes (6+)
				string: navigator.userAgent,
				subString: "Netscape",
				identity: "Netscape"
			},
			{
				string: navigator.userAgent,
				subString: "MSIE",
				identity: "Explorer",
				versionSearch: "MSIE"
			},
			{
				string: navigator.userAgent,
				subString: "Trident",
				identity: "Explorer",
				versionSearch: "rv"
			},
			{
				string: navigator.userAgent,
				subString: "Gecko",
				identity: "Mozilla",
				versionSearch: "rv"
			},
			{ 		// for older Netscapes (4-)
				string: navigator.userAgent,
				subString: "Mozilla",
				identity: "Netscape",
				versionSearch: "Mozilla"
			}
		],
		dataOS : [
			{
				string: navigator.platform,
				subString: "Win",
				identity: "Windows"
			},
			{
				string: navigator.platform,
				subString: "Mac",
				identity: "Mac"
			},
			{
				   string: navigator.userAgent,
				   subString: "iPhone",
				   identity: "iPhone/iPod"
		    },
			{
				string: navigator.platform,
				subString: "Linux",
				identity: "Linux"
			}
		]
	
	};
	BrowserDetect.init();
}

function detectFirebug(){
	if( window.console && ( window.console.firebug || console.firebug ) ){
		document.getElementById("divFBWarning").style.display = 'block';
	}
}

function ready(){
	detectFirebug();
	detectBrowser();
	if( checkBrowser() ){
		if( document.getElementById("username").value.length > 0 ){
			document.getElementById("password").focus();
		}else{
			document.getElementById("username").focus();
		}
		document.getElementById("Enterprise").value = getEnterpriseCode();
	}
	loadXMLDoc();
}