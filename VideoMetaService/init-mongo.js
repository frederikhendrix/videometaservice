db.createUser({
    user: 'admin',
    pwd: 'Test123!',
    roles: [
        {
            role: 'admin',
            db: 'VideoMetaDB'
        }
    ]
});