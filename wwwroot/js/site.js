//jQuery to collapse the navbar on scroll
$(window).scroll(function() {
    if ($(".navbar").offset().top > 50) {
        $(".navbar-fixed-top").addClass("top-nav-collapse");
    } else {
        $(".navbar-fixed-top").removeClass("top-nav-collapse");
    }
});


$(function() {

    //jQuery for page scrolling feature - requires jQuery Easing plugin
    $('a.page-scroll').bind('click', function(event) {
        var $anchor = $(this);
        $('html, body').stop().animate({
            scrollTop: $($anchor.attr('href')).offset().top
        }, 1500, 'easeInOutExpo');
        event.preventDefault();
    });

    // run clocks
    runClock('#currentGameTimeLeft');
    runClock('#estimatedGameTimeLeft');

    refreshPlayerLists()
});

function selectedPlayers(first, second) {
    $(first).on('changed.bs.select', function (e, clickedIndex, newValue, oldValue) {
        console.log(clickedIndex);
        console.log(newValue);
        console.log(oldValue);
        
        var element = $(this).parent().find("div li a")[clickedIndex].text;
        var name = element.split(' ')[0];
        var query = "div li a:contains('" + name + "')";
        if(newValue)
        {
            $(second).parent().find(query).hide();
        } else {
            $(second).parent().find(query).show();
        }
        
    });
};

function refreshPlayerLists() {
    var picker = $('.selectpicker');
    if(picker.length > 0) {
        selectedPlayers('#firstTeamSelect', '#secondTeamSelect');
        selectedPlayers('#secondTeamSelect', '#firstTeamSelect');
        // picker.selectpicker('refresh');
    }
}

function createGame(actionUrl) {
    var postData = {
        "FirstTeam": $("#firstTeamSelect").val(),
        "SecondTeam": $("#secondTeamSelect").val()
    };
    $.post(actionUrl, postData, function(result){
        $("#gameDiv").html(result);
        refreshPlayerLists();
    });
}

function runClock(clockName) {
    var clock = $(clockName).FlipClock(
        $(clockName).text(), 
        {
            autoStart: false,
            countdown: true,
            clockFace: 'MinuteCounter'
        });
    clock.start();
}