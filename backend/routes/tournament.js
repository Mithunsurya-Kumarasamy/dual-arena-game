const express = require("express");
const router = express.Router();
const db = require("../db");


router.post("/saveMatch", (req, res) => {
    const { tournamentId, matchIndex, winner } = req.body;

    console.log("=== SAVE MATCH ===");
    console.log("TID:", tournamentId);
    console.log("Index:", matchIndex);
    console.log("Winner:", winner);

    const sql = `
        UPDATE TournamentMatches
        SET Winner = ?
        WHERE TournamentID = ? AND MatchOrder = ?
    `;

    db.query(sql, [winner, tournamentId, matchIndex], (err, result) => {
        if (err) {
            console.log("❌ DB ERROR:", err);
            return res.send({ success: false });
        }

        console.log("✅ Updated match");
        res.send({ success: true });
    });
});

module.exports = router;