const express = require("express");
const router = express.Router();
const db = require("../db");


// 🥇 TOP PLAYERS
router.get("/topPlayers", (req, res) => {
    db.query("CALL GetTopPlayers()", (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});


// 📜 MATCH HISTORY
router.get("/matchHistory", (req, res) => {
    db.query("CALL GetMatchHistory()", (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});


// 🗺️ MOST PLAYED MAP
router.get("/mostPlayedMap", (req, res) => {
    db.query("CALL GetMostPlayedMap()", (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});


// 🏆 TOURNAMENT STATS
router.get("/tournamentStats", (req, res) => {
    db.query("CALL GetTournamentStats()", (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});

// 👤 INDIVIDUAL PLAYER STATS
router.get("/playerStats/:username", (req, res) => {
    const username = req.params.username;

    db.query("CALL GetPlayerStats(?)", [username], (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});

// 📊 WIN RATE
router.get("/winRate", (req, res) => {
    db.query("CALL GetWinRate()", (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});

// 🎯 MOST USED MOVE
router.get("/mostUsedMove/:username", (req, res) => {
    const username = req.params.username;

    db.query("CALL GetMostUsedMove(?)", [username], (err, result) => {
        if (err) {
            console.log(err);
            res.send([]);
        } else {
            res.send(result[0]);
        }
    });
});

module.exports = router;
