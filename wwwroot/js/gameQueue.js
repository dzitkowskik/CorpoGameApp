$(function() {

    var chat = $.connection.gameQueueHub;

    chat.client.playerJoined = function (newUser) {
        alert("playerJoined");
        console.log(newUser);
    }

    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        $("#SearchGameJoinGameQueue").click(function () {
            alert("try to join game");
            chat.server.joinQueue($('#SeachGameCurrentPlayerId').val());
        });
    });

});