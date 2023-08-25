<?php

    $con = mysqli_connect('localhost', 'root', 'root', 'unityaccess');

    if(mysqli_connect_errno())
    {
        echo "1: Connection failed";
        exit();
    }

    $username = $_POST["username"];
    $score = $_POST["score"];

    // Check if username already exists
    $usernamecheckquery = "SELECT username, score FROM highscore WHERE username = '" . $username . "';";

    $updatequery = "UPDATE highscore SET score = '{$score}' WHERE username = '{$username}';";

    $usernamecheck = mysqli_query($con, $usernamecheckquery) or die("2: Name check query failed");

    if (mysqli_num_rows($usernamecheck) > 0) 
    {
        while($row = mysqli_fetch_array($usernamecheck))
        {
            if($row["score"] < $score)
            {
                $updatescore = mysqli_query($con, $updatequery) or die("5: Update score failed");
            }
        }
    }
    else
    {
        //Add new player
        $insertuserquery = "INSERT INTO highscore (username, score) VALUES ('{$username}', {$score});";
        mysqli_query($con, $insertuserquery) or die("4: Insert player score query failed");
    }

    // Successful register - Return data to show on ranking
    $selectrankingquery = "SELECT username, score FROM `highscore` ORDER BY score DESC LIMIT 10;";
    $selectranking = mysqli_query($con, $selectrankingquery) or die("Failed to retrieve ranking");
    
    while ($row = mysqli_fetch_array($selectranking)) 
    {
        echo("{$row['username']},{$row['score']}*");
    }

    $playerbestscorequery = "SELECT score FROM highscore WHERE username = '{$username}';";
    // echo(" {$playerbestscorequery}");
    $playerbestscore = mysqli_query($con, $playerbestscorequery) or die("Failed to retrieve best score");

    while ($row = mysqli_fetch_array($playerbestscore)) 
    {
        echo("{$row['score']}");
    }

    // echo("0");

?>