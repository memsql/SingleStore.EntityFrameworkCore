import json
import os
import sys
from s2ms_cluster import WORKSPACE_ENDPOINT_FILE


if __name__ == "__main__":

    home_dir = os.getenv("HOMEPATH")
    if home_dir is None:
        home_dir = os.getenv("HOME")

    with open(WORKSPACE_ENDPOINT_FILE, "r") as f:
        hostname = f.read()
    password = os.getenv("SQL_USER_PASSWORD")

    with open("./.circleci/config.json", "r") as f_in:
        config_content = json.load(f_in)

    config_content["Data"]["ConnectionString"] = config_content["Data"]["ConnectionString"].replace("SINGLESTORE_HOST", hostname, 1)
    config_content["Data"]["ConnectionString"] = config_content["Data"]["ConnectionString"].replace("SQL_USER_PASSWORD", password, 1)
    config_content["Data"]["ConnectionString"] = config_content["Data"]["ConnectionString"].replace("SQL_USER_NAME", "admin", 1)

    config_content["ManagedService"] = True

    if len(sys.argv) < 1:
        print("Not enough arguments to fill the config file!")
        exit(1)

    test_block = sys.argv[1]

    with open(f"test/{test_block}/config.json", "w") as f_out:
        json.dump(config_content, f_out, indent=4)

    with open(os.path.join(home_dir, "CONNECTION_STRING"), "w") as f_conn:
        f_conn.write(config_content["Data"]["ConnectionString"])
