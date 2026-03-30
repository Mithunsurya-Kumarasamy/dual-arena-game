const express = require("express");
const router = express.Router();
const db = require("../db");

// CREATE MATCH
router.post("/create", (req, res) => {
    const { p1, p2, map } = req.body;

    const sql = `
        INSERT INTO Matches (Player1ID, Player2ID, MapID)
        VALUES (?, ?, ?)
    `;

    db.query(sql, [p1, p2, map], (err, result) => {
        if (err) throw err;

        res.send({ success: true, matchID: result.insertId });
    });
});

// UPDATE WINNER
router.post("/winner", (req, res) => {
    const { matchID, winnerID } = req.body;

    const sql = `
        UPDATE Matches SET WinnerID=? WHERE MatchID=?
    `;

    db.query(sql, [winnerID, matchID], (err) => {
        if (err) throw err;

        res.send({ success: true });
    });
});

module.exports = router;