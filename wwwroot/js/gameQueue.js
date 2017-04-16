$(function() {

    var chat = $.connection.gameQueueHub;

    chat.client.playerJoined = function (newUser) {

        var newLiElem = '<li class="list-group-item">';
        newLiElem += newUser.Name + ' ' + newUser.Surname;
        newLiElem += '(score: ' + newUser.Score + ')';
        newLiElem += '</li>';

        $('#QueuedPlayersListDiv .list-group').append(newLiElem);
    }

    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        $("#SearchGameJoinGameQueue").click(function () {
            chat.server.joinQueue($('#SeachGameCurrentPlayerId').val());
        });
    });

});