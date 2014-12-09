$(document).ready(function(){
	
	selectbox('.selectbox');	
	
//Make whole format box clickable
	$('ul.formatlist li').click(function() {
		var url = $(this).find('a').attr('href');
		window.location = url;
	});

	//Toggle listgroup collapsible content
	$('.toggle_link').click(function() {
		//Find and toggle the closest collapsible element.
		$(this).toggleClass('open').next('.collapsible_content').slideToggle('fast');
		return false;
	});
	
});

function toggleLoginBox() {
	$('.login_form').toggle('fast');
	return false;
}	

function selectbox(e)
{
	var element = $(e);
	var mouse_is_inside = false;
	
	// set up each electbox
		
	element.each(function(index) {
		var selectbox = 
			{
				height: $(this).outerHeight(),
				width: $(this).outerWidth()
			} 
		
		$(this).find('ul').css({
			//top: selectbox.height,
			width: selectbox.width
		});
	
	if (window.console && console.log) {
		console.log('index: ' +index + ', outerheight: ' + selectbox.height);}
	}); 
		
	element.find('> a').click(function(el) {
		element.removeClass('open');
		$(this).parent().addClass('open');
		
	});
	
	element.find('li a').click(function(el) {
		element.removeClass('open');
		$(this).parent().parent().parent().find('> a').text($(this).text());
	});
	
    element.hover(function(){ 
        mouse_is_inside=true; 
    }, function(){ 
        mouse_is_inside=false; 
    });
	
    $("body").mouseup(function(){ 
        if(! mouse_is_inside) element.removeClass('open');
    });

}

/* EOF */