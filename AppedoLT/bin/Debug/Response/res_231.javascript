var geocoder;
var map;
var member_markers = new Array();
var marker = new Array();

function initialize() {
	//geocoder = new google.maps.Geocoder();
	var latlng = new google.maps.LatLng(47.57653, 8.96484);
	var myOptions = {
		zoom: 3,
		center: latlng,
		disableDefaultUI: true,
		styles: theme,			
		mapTypeId: google.maps.MapTypeId.ROADMAP,
		draggable: true,
		disableDoubleClickZoom: false,
		scrollwheel: false,
		backgroundColor: '#ededed'
	};
	
	map = new google.maps.Map(document.getElementById('map_canvas'), myOptions);
	panOffset(750,300);

	
	google.maps.event.addListener(map, 'zoom_changed', function() {
		//panOffset();
	});	
}

function panOffset(pX, pY) {

	// set default
	var offset = {x:880, y:205}; //710 centers on content area
	
	if (pX) offset.x = pX;
	if (pY) offset.y = pY;	
	
	map.panBy((window.innerWidth/2)-offset.x,(window.innerHeight/2)-offset.y);
}

function zoom(way) {
	var currentZoomLevel = map.getZoom();	
	if(way == "in")
	{
		
		console.log("Zooming in from level "+currentZoomLevel);
		map.setZoom(currentZoomLevel+1);	
	}	
	else if(way == "out")
	{
		console.log("Zooming out from level "+currentZoomLevel);
		map.setZoom(currentZoomLevel-1);	
	}	
}

function memberMarkers() {
	//var marker, i;
	//var bounds = new google.maps.LatLngBounds();
	var iterator = 0;

	var image = new google.maps.MarkerImage('_img/map_marker_std.png', //http://lab.fredrikdanielsson.se/client/sparksnetwork/www/
		// This marker is 20 pixels wide by 32 pixels tall.
		new google.maps.Size(32, 33),
		// The origin for this image is 0,0.
		new google.maps.Point(0,0),
		// The anchor for this image is the base of the flagpole at 0,32.
		new google.maps.Point(11, 33));
	
	var shadow = new google.maps.MarkerImage('_img/map_marker_shadow.png',
		// The shadow image is larger in the horizontal dimension
		// while the position and offset are the same as for the main image.
		new google.maps.Size(20, 20),
		new google.maps.Point(0,0),
		new google.maps.Point(20,20));		
	for (var i=0; i < member.length; i++) {
		//console.log ('looking up ' +member[i].city + ', '+ member[i].country);
		/* setTimeout(function() {
			console.log('test');
			addMarker();
		}, i*50); */
		addMarker();
	}
	
	function addMarker()
	{
		console.log('iterator: '+iterator);
		var markerLatLang = new google.maps.LatLng(member[iterator].lat, member[iterator].lang);
		var markerMemberName = member[iterator].name;
		
		marker[iterator] = new google.maps.Marker({
			map: map,
			icon:image,
			shadow:shadow,
			position: markerLatLang,
			title: markerMemberName,
			/*animation: google.maps.Animation.DROP, /* SWITCH */
			index: iterator
		});
		
		member_markers.push(marker);
		//bounds.extend(markerLatLang);
		//map.fitBounds(bounds);
	
		
		google.maps.event.addListener(marker[iterator], 'click', function() {
			console.log(this.index);
			moveToMarker(this.index);
		});

		iterator++;
	}	
}

function clearMarkers() {
  if (marker) {
    for (i in marker) {
      marker[i].setMap(null);
    }
    marker.length = 0;
  }
}

function moveToMarker(index) 
{
	//var lat = member[index].lat;
	//var lang = member[index].lang;	
	var latlng = marker[index].getPosition(); //new google.maps.LatLng(lat, lang);
	
	map.panTo(latlng);
	map.setZoom(5);	
	panOffset();
	highlightMarker(index);
	google.maps.event.addListenerOnce(map, 'idle', function() {
		console.log('idle now!')
		openMemberPage(index);
	});
}

var old_member_markers_index = -1;
function highlightMarker(index)
{
	var std = marker[index].getIcon();
	
	var highlight = new google.maps.MarkerImage('_img/map_marker_highlight.png', //http://lab.fredrikdanielsson.se/client/sparksnetwork/www/
		// This marker is 20 pixels wide by 32 pixels tall.
		new google.maps.Size(32, 33),
		// The origin for this image is 0,0.
		new google.maps.Point(0,0),
		// The anchor for this image is the base of the flagpole at 0,32.
		new google.maps.Point(11, 33));

	console.log('marker: ' + index);
	
	if (old_member_markers_index != -1)
	{
		marker[old_member_markers_index].setIcon(std);
	}	
	
	marker[index].setIcon(highlight);
	//marker[index].setAnimation(google.maps.Animation.BOUNCE);

	old_member_markers_index = index;
}


function openMemberPage(index)
{
	$('.member_list_title').hide();
	$('#custommapcontrol').hide();	
	$('.member_page_container').removeClass('init');

	$('.member_page_container').one('transitionend', function( event ) {
		openMemberPage();
	});
	$('.member_page_container').one('webkitTransitionEnd', function( event ) {
		openMemberPage();
	});
	$('.member_page_container').one('oTransitionEnd', function( event ) {
		openMemberPage();	
	});	
	
	function openMemberPage() {
		$('.member_page').fadeIn()
		$('.member_page_container').removeClass('animate');
	}
}



var theme = [
	{
		featureType: "water",
		stylers: [
			{ saturation: -100 },
			{ lightness: 60 }
		]
	},{
		featureType: "water",
	    elementType: "labels",		
		stylers: [
			{ visibility: "off" }
		]
	},{	
		featureType: "road",
		stylers: [
			{ saturation: -100 },
			{ lightness: 66 },
			{ visibility: "off" }
		]
	},{
		featureType: "transit",
		stylers: [
			//{ saturation: -100 },
			//{ invert_lightness: true },
			//{ lightness: 74 },
			{ visibility: "off" }
		]
	},{
		featureType: "poi",
		stylers: [
			{ saturation: -100 },
			{ lightness: 66 },
			{ visibility: "simplified" }
		]
	},{
    	featureType: "poi",
    	elementType: "geometry",
		stylers: [
			{ visibility: "off" }
		]
	},{
		featureType: "landscape.natural",
		stylers: [
			{ saturation: 100 },
			{Hue: "#F8A331" },
			{Gamma:	0.08 }	
		]
	},{
		featureType: "landscape.man_made",
		stylers: [
			{saturation: -100 },
			{ lightness: 20 }
		]
	},{
		featureType: "administrative",
		stylers: [
			{ saturation: -100 },
			{ lightness: 64 },
			{ visibility: "on" }
		]
	},{
		featureType: "administrative.province",
		stylers: [
			{ visibility: "off" }
		]
	},{
		featureType: "administrative.locality",
		stylers: [
			{ visibility: "simplified" }
		]
	},{
		featureType: "administrative.country",
	    elementType: "labels",		
		stylers: [
			//{ invert_lightness: true },
			{ lightness: 60 }
		]
	}
];

var member = [
	{
		name: "Amygdala",
		city: "Rome",
		country: "italy",
		lat: 41.89052,
		lang:12.49425
	},{
		name: "Atm Grupa",
		city: 	"Warszawa",
		country: "Poland",
		lat:52.22968,
		lang:21.01223
	},{
		name: "Atm Grupa",
		city: 	"Wrozlowskie",
		country: "Poland",
		lat: 50.79875,
		lang:16.84715
	},{	
		name: "Ay Yapim",
		city: "Istanbul",
		country: "Turkey",
		lat: 41.00527,
		lang:28.97696
	},{
		name: "Collaboration Inc",
		city: "Tokyo",
		country: "Japan",
		lat: 35.68949,
		lang:139.69171
	},{
		name: "Elephant & C:ie",
		city: "Paris",
		country: "France",
		lat: 48.85661,
		lang:2.35222
	},{
		name: "Everyshow",
		city: "Seoul",
		country: "Korea",
		lat: 37.56654,
		lang:126.97797
	},{
		name: "FILM.UA",
		city: "Kiew",
		country: "Ukraine",
		lat: 50.45010,
		lang:30.52340
	},{
		name: "Idea Asia Media",
		city: "Shanghai",
		country: "China",
		lat: 31.23039,
		lang:121.47370
	},{
		name: "ICM Talent",
		city: "Los Angeles",
		country: "USA",
		lat: 34.05223,
		lang:-118.24368
	},{
		name: "ICM Talent",
		city: "New York",
		country: "USA",
		lat: 40.71435,
		lang:-74.00597
	},{
		name: "ICM Talent",
		city: "London",
		country: "United Kingdom",
		lat: 51.50015,
		lang:-0.12624
	},{
		name: "Fireball SEM",
		city: "Cape Town",
		country: "South Africa",
		lat: -33.92487,
		lang:18.42406
	},{
		name: "Imagic",
		city: "Beirut",
		country: "Lebanon",
		lat: 33.88863,
		lang:35.49548
	},{
		name: "Kapa Studios",
		city: "Athens",
		country: "Greece",
		lat: 37.97918,
		lang:23.71665
	},{
		name: "Lemon Productions",
		city: "Jakarta",
		country: "Indonesia",
		lat: -6.21154,
		lang:106.84517
	},{
		name: "OTW",
		city: "Stockholm",
		country: "Sweeden",
		lat: 59.33279,
		lang:18.06449
	},{
		name: "south&browse",
		city: "Munich",
		country: "Germany",
		lat: 48.13913,
		lang:11.58019
	},{
		name: "south&browse",
		city: "Berlin",
		country: "Germany",
		lat: 52.52341,
		lang:13.41140
	},{
		name: "Somos Zebra",
		city: "Miami",
		country: "USA",
		lat: 25.78897,
		lang:-80.22644
	},{
		name: "Sputnik TV",
		city: "Berchem",
		country: "Belgium",
		lat: 50.79018,
		lang:3.50940
	},{
		name: "Susamuru",
		city: "Helsinki",
		country: "Finland",
		lat: 60.16981,
		lang:24.93824
	},{
		name: "Trio Orange",
		city: "Montréal",
		country: "Canada",
		lat: 45.50867,
		lang:-73.55399
	},{
		name: "RingierTV",
		city: "Zürich",
		country: "Switzerland",
		lat: 47.36735,
		lang:8.55000
	},{
		name: "Zebra Producciones",
		city: "Madrid",
		country: "Spain",
		lat: 41.89052,
		lang:-3.70035
	},{
		name: "Zebra Producciones",
		city: "Barcelona",
		country: "Spain",
		lat: 41.38792,
		lang:2.16992
	}
];

function loadScript() {
	var script = document.createElement('script');
	script.type = 'text/javascript';
	script.src = 'http://maps.googleapis.com/maps/api/js?sensor=false&' + 'callback=initialize';
	document.body.appendChild(script);
}

//window.onload = loadScript;
