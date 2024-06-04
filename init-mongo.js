db = db.getSiblingDB('Squares');

db.createUser(
    {
        user: "root",
        pwd: "pwd1",
        roles: [
            {
                role: "readWrite",
                db: "Squares"
            }
        ]
    }
);

db.createCollection("Points");

db.Points.insert({
  "X": "2",
  "Y": "-2"
});