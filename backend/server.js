const express = require("express");
const cors = require("cors");

const app = express();

app.use(cors());
app.use(express.json());
app.use(express.urlencoded({ extended: true }));
// routes
app.use("/auth", require("./routes/auth"));
app.use("/match", require("./routes/match"));
app.use("/stats", require("./routes/stats"));


app.listen(3000, () => {
    console.log("Server running on port 3000");
});