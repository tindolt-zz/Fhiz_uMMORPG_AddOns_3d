// =======================================================================================
// Created and maintained by iMMO
// Usable for both personal and commercial projects, but no sharing or re-sale
// * Discord Support Server.............: https://discord.gg/YkMbDHs
// * Public downloads website...........: https://www.indie-mmo.net
// * Pledge on Patreon for VIP AddOns...: https://www.patreon.com/IndieMMO
// * Instructions.......................: https://indie-mmo.net/knowledge-base/
// =======================================================================================
using System.Collections.Generic;

#if _MYSQL
using MySql.Data;								// From MySql.Data.dll in Plugins folder
using MySql.Data.MySqlClient;                   // From MySql.Data.dll in Plugins folder
#elif _SQLITE

using SQLite; 						// copied from Unity/Mono/lib/mono/2.0 to Plugins

#endif

// DATABASE (SQLite / mySQL Hybrid)

public partial class Database
{
    // -----------------------------------------------------------------------------------
    // Connect_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("Connect")]
    private void Connect_UCE_AccountUnlockables()
    {
#if _MYSQL
		ExecuteNonQueryMySql(@"CREATE TABLE IF NOT EXISTS account_unlockables (
 			account VARCHAR(32) NOT NULL,
 			unlockable VARCHAR(32) NOT NULL
 		)");
#elif _SQLITE
        connection.CreateTable<account_unlockables>();
#endif
    }

    // -----------------------------------------------------------------------------------
    // UCE_GetAccountUnlockables
    // -----------------------------------------------------------------------------------
    public List<string> UCE_GetAccountUnlockables(string accountName)
    {
        List<string> unlockables = new List<string>();

#if _MYSQL
		var table = ExecuteReaderMySql("SELECT unlockable FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", accountName));
        foreach (var row in table)
            unlockables.Add((string)row[0]);
#elif _SQLITE
        var table = connection.Query<account_unlockables>("SELECT unlockable FROM account_unlockables WHERE account=?", accountName);
        foreach (var row in table)
            unlockables.Add(row.unlockable);
#endif
        return unlockables;
    }

    // -----------------------------------------------------------------------------------
    // CharacterLoad_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterLoad")]
    private void CharacterLoad_UCE_AccountUnlockables(Player player)
    {
#if _MYSQL
		var table = ExecuteReaderMySql("SELECT unlockable FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", player.account));
        foreach (var row in table)
            player.UCE_accountUnlockables.Add((string)row[0]);
#elif _SQLITE
        var table = connection.Query<account_unlockables>("SELECT unlockable FROM account_unlockables WHERE account=?", player.account);
        foreach (var row in table)
            player.UCE_accountUnlockables.Add(row.unlockable);
#endif
    }

    // -----------------------------------------------------------------------------------
    // CharacterSave_UCE_AccountUnlockables
    // -----------------------------------------------------------------------------------
    [DevExtMethods("CharacterSave")]
    private void CharacterSave_UCE_AccountUnlockables(Player player)
    {
#if _MYSQL
		ExecuteNonQueryMySql("DELETE FROM account_unlockables WHERE `account`=@account", new MySqlParameter("@account", player.account));
		for (int i = 0; i < player.UCE_accountUnlockables.Count; ++i) {
			ExecuteNonQueryMySql("INSERT INTO account_unlockables VALUES (@account, @unlockable)",
 				new MySqlParameter("@account", player.account),
 				new MySqlParameter("@unlockable", player.UCE_accountUnlockables[i]));
 		}
#elif _SQLITE
        connection.Execute("DELETE FROM account_unlockables WHERE account=?", player.account);
        for (int i = 0; i < player.UCE_accountUnlockables.Count; ++i)
            connection.Insert(new account_unlockables
            {
                account = player.account,
                unlockable = player.UCE_accountUnlockables[i]
            });
#endif
    }

    // -----------------------------------------------------------------------------------
}