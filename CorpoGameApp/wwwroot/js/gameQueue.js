$(function() {

    let connection = new signalR.HubConnection('/GameQueueHub');

    togglePlayersList();

    function togglePlayersList() {
        if ($('#currentPlayersList').children().length > 0) {
            $('#CurrentlyPlayingPlayersListDiv').show();
        } else {
            $('#CurrentlyPlayingPlayersListDiv').hide();
        }
        if ($('#queuedPlayersList').children().length > 0) {
            $('#QueuedPlayersListDiv').show();
        } else {
            $('#QueuedPlayersListDiv').hide();
        }
    }

    function appendPlayerToList(player, list) {
        var currentPlayerId = $('#SeachGameCurrentPlayerId').val();

        var playerString = '{0} {1} (score: {2})'
            .format(player.Name, player.Surname, player.Score);

        var newItem = $("<li>")
            .addClass('list-group-item')
            .text(playerString);

        if (player.Id === currentPlayerId) {
            newItem.addClass('list-group-item-success')
        }

        list.append(newItem);
        togglePlayersList();
    }

    function fillPlayerList(players, list) {
        if (players != null) {
            for (var i = 0; i < players.length; i++) {
                var player = players[i];
                appendPlayerToList(player, list);
            }
        }
    }

    function getEstimatedTimeToPlay(players, gameDuration, currentTimeLeft, capacity) {
        var currentPlayerId = $('#SeachGameCurrentPlayerId').val();

        for (var i = 0; i < players.length; i++) {
            if (players[i].Id == currentPlayerId) {
                break;
            }
        }

        var gamesLeft = i / capacity
        return currentTimeLeft + gameDuration * gamesLeft;
    }

    function updateTeamQueueList(searchGameViewModel) {
        console.log(searchGameViewModel);

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
        var currentTimeLeft = 0;
        if (searchGameViewModel.CurrentGameTimeLeft != null) {
            currentTimeLeft = searchGameViewModel.CurrentGameTimeLeft.secondsLeft;
            $('#currentGameTimeLeft').val(currentTimeLeft);
            $('#currentGameTimeLeft').siblings().find("label").val(searchGameViewModel.CurrentGameTimeLeft.Label);
        }
        if (searchGameViewModel.EstimatedGameTimeLeft != null) {
            $('#estimatedGameTimeLeft').val(
                getEstimatedTimeToPlay(
                    queuedPlayers,
                    searchGameViewModel.GameDurationInSeconds,
                    currentTimeLeft,
                    searchGameViewModel.GameCapacity));
        }
    }

    connection.on('refresh', function() {
        location.reload();
    });

    connection.on('updateTeamQueueList', data => updateTeamQueueList(data));

    connection.start().then(() => {
        $("#SearchGameJoinGameQueue").click(function() {
            connection.invoke('joinQueue', $('#SeachGameCurrentPlayerId').val());
        });
        $("#LeaveQueueButtnon").click(function() {
            connection.invoke('leaveQueue', $('#SeachGameCurrentPlayerId').val());
        });   
    });
});