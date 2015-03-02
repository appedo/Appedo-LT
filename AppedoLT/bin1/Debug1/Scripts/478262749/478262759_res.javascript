/**
 * load JS or CSS to HEAD tag of HTML
 */
function loadjscssfile(filename, filetype){
	if (libFilesAdded.indexOf("["+filename+"]")!=-1)
		return false;
	
	if (filetype=="js"){ //if filename is a external JavaScript file
		var fileref=document.createElement('script')
		fileref.setAttribute("type","text/javascript")
		fileref.setAttribute("src", filename)
	}
	else if (filetype=="css"){ //if filename is an external CSS file
		var fileref=document.createElement("link")
		fileref.setAttribute("rel", "stylesheet")
		fileref.setAttribute("type", "text/css")
		fileref.setAttribute("href", filename)
	}
	if (typeof fileref!="undefined")
		document.getElementsByTagName("head")[0].appendChild(fileref)
	libFilesAdded += "["+filename+"]"
}

/**
 * Change Icon in browser window
 */
function changeProjectIcon(){
	var fileref = document.createElement("link")
	fileref.setAttribute("rel", "shortcut icon")
	fileref.setAttribute("type", "image/x-icon")
	fileref.setAttribute("href", "./images/q.ico")
	document.getElementsByTagName("head")[0].appendChild(fileref)
}
changeProjectIcon();