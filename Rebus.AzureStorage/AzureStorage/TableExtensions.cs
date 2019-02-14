﻿//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Microsoft.WindowsAzure.Storage;
//using Microsoft.WindowsAzure.Storage.Table;

//namespace Rebus.AzureStorage
//{
//    static class TableExtensions
//    {
//        public static async Task<IList<T>> ExecuteQueryAsync<T>(this CloudTable table, TableQuery<T> query, TableRequestOptions requestOptions, OperationContext operationContext) where T : ITableEntity, new()
//        {

//            var items = new List<T>();
//            TableContinuationToken token = null;

//            do
//            {

//                TableQuerySegment<T> seg = await table.ExecuteQuerySegmentedAsync<T>(query, token, requestOptions, operationContext);
//                token = seg.ContinuationToken;
//                items.AddRange(seg);


//            } while (token != null );

//            return items;
//        }
//    }
//}
