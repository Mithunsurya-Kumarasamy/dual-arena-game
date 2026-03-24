const express = require("express");
const router = express.Router();
const db = require("../db");

// REGISTER
router.post("/register", (req, res) => {
    const { username, password } = req.body;

    const sql = "INSERT INTO Users (Username, Password) VALUES (?, ?)";

    db.query(sql, [username, password], (err, result) => {
        if (err) {
            res.send({ success: false, message: "Username already exists" });
        } else {
            res.send({ success: true });
        }
    });
});

// LOGIN
router.post("/login", (req, res) => {
    const { username, password } = req.body;

    const sql = "SELECT * FROM Users WHERE Username=? AND Password=?";

    db.query(sql, [username, password], (err, result) => {
        if (err) throw err;

        if (result.length > 0) {
            res.send({ success: true, user: result[0] });
        } else {
            res.send({ success: false });
        }
    });
});

module.exports = router;