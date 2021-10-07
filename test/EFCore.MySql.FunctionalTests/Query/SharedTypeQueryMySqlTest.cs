﻿// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.TestUtilities;
using Pomelo.EntityFrameworkCore.MySql.FunctionalTests.TestUtilities;

namespace Pomelo.EntityFrameworkCore.MySql.FunctionalTests.Query
{
    public class SharedTypeQueryMySqlTest : SharedTypeQueryRelationalTestBase
    {
        protected override ITestStoreFactory TestStoreFactory => MySqlTestStoreFactory.Instance;

        public override async Task Can_use_shared_type_entity_type_in_query_filter(bool async)
        {
            await base.Can_use_shared_type_entity_type_in_query_filter(async);

            AssertSql(
                @"SELECT `v`.`Value`
FROM `ViewQuery24601` AS `v`
WHERE EXISTS (
    SELECT 1
    FROM `STET` AS `s`
    WHERE (`s`.`Value` = `v`.`Value`) OR (`s`.`Value` IS NULL AND (`v`.`Value` IS NULL)))");
        }

        public override async Task Can_use_shared_type_entity_type_in_query_filter_with_from_sql(bool async)
        {
            await base.Can_use_shared_type_entity_type_in_query_filter_with_from_sql(async);

            AssertSql(
                @"SELECT `v`.`Value`
FROM `ViewQuery24601` AS `v`
WHERE EXISTS (
    SELECT 1
    FROM (
        Select * from STET
    ) AS `s`
    WHERE (`s`.`Value` = `v`.`Value`) OR (`s`.`Value` IS NULL AND (`v`.`Value` IS NULL)))");
        }
    }
}
