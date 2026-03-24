const express = require("express");
const router = express.Router();
const db = require("../db");

// GET PLAYER STATS
router.get("/player/:id", (req, res) => {
    const userID = req.params.id;

    const sql = "SELECT * FROM PlayerStats WHERE UserID=?";

    db.query(sql, [userID], (err, result) => {
        if (err) throw err;

        res.send(result[0]);
    });
});

// UPDATE STATS
router.post("/update", (req, res) => {
    const { userID, win } = req.body;

    let sql;

    if (win) {
        sql = `
        UPDATE PlayerStats 
        SET Wins = Wins + 1, TotalMatches = TotalMatches + 1
        WHERE UserID = ?
        `;
    } else {
        sql = `
        UPDATE PlayerStats 
        SET Losses = Losses + 1, TotalMatches = TotalMatches + 1
        WHERE UserID = ?
        `;
    }

    db.query(sql, [userID], (err) => {
        if (err) throw err;

        res.send({ success: true });
    });
});

module.exports = router;