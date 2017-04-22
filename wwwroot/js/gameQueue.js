$(function() {

    var chat = $.connection.gameQueueHub;

    var currentPlayerId = 0;

    function appendPlayerToList(player, list) {
        var playerString = '{0} {1} (score: {2})'
            .format(player.Name, player.Surname, player.Score);
        
        var newItem = $("<li>")
            .addClass("list-group-item")
            .text(playerString)

        if(player.Id === currentPlayerId) {
            newItem.addClass('list-group-item-info')
        }

        list.append(newItem);
    }

    function fillPlayerList(players, list) {
        if(players != null) {
            for(var i = 0; i < players.length; i++) {
                var player = players[i];
                appendPlayerToList(player, list);
            }
        }
    }

    chat.client.updateTeamQueueList = function (searchGameViewModel) {
        console.log(searchGameViewModel);

        currentPlayerId = searchGameViewModel.CurrentPlayerId;

        var currentPlayersListElement = $('#currentPlayersList');
        var queuedPlayersListElement = $('#queuedPlayersList');

        // clear player lists
        currentPlayersListElement.empty();
        queuedPlayersListElement.empty();
        
        // update lists
        var currentPlayers = searchGameViewModel.CurrentlyPlayingPlayers;
        var queuedPlayers = searchGameViewModel.QueuedPlayers;
        fillPlayerList(currentPlayers, currentPlayersListElement);
        fillPlayerList(queuedPlayers, queuedPlayersListElement);

        // update time left
        if(searchGameViewModel.CurrentGameTimeLeft != null) {
            $('#currentGameTimeLeft').val(searchGameViewModel.CurrentGameTimeLeft.secondsLeft);
            $('#currentGameTimeLeft').siblings().find("label").val(searchGameViewModel.CurrentGameTimeLeft.Label);
        }
        if(searchGameViewModel.EstimatedGameTimeLeft != null) {
            $('#estimatedGameTimeLeft').val(searchGameViewModel.EstimatedGameTimeLeft.secondsLeft);
            $('#estimatedGameTimeLeft').siblings().find("label").val(searchGameViewModel.EstimatedGameTimeLeft.Label);
        }
    }

    $.connection.hub.logging = true;
    $.connection.hub.start().done(function () {
        $("#SearchGameJoinGameQueue").click(function () {
            chat.server.joinQueue($('#SeachGameCurrentPlayerId').val());
        });
    });


    String.prototype.format = String.prototype.f = function() {
        var s = this,
            i = arguments.length;

        while (i--) {
            s = s.replace(new RegExp('\\{' + i + '\\}', 'gm'), arguments[i]);
        }
        return s;
    };
});