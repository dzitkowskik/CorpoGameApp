

chat.client.playerJoined = function (newUser) {
    alert("playerJoined");
    console.log(newUser);
}

$("#SearchGameJoinGameQueue").click(function () {
    alert("try to join game");
    chat.server.send("JoinQUeue", $('#SeachGameCurrentPlayerId').val());
});

var chat = $.connection.chatHub;
$.connection.hub.logging = true;
$.connection.hub.start().done(function () {
    chat.server.connect("test");
});