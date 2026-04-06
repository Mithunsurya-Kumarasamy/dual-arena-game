const express = require("express");
const router = express.Router();
const db = require("../db");


router.get("/:type", (req, res) => {
    const type = req.params.type;

    db.query("CALL GetRules(?)", [type], (err, result) => {
        if (err) {
            console.log(err);
            return res.send([]);
        }

        res.send(result[0][0]); // single object
    });
});

module.exports = router;
