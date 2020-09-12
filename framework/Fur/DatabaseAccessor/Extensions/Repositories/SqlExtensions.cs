﻿// -----------------------------------------------------------------------------
// Fur 是 .NET 5 平台下极易入门、极速开发的 Web 应用框架。
// Copyright © 2020 Fur, Baiqian Co.,Ltd.
//
// 框架名称：Fur
// 框架作者：百小僧
// 框架版本：1.0.0
// 源码地址：https://gitee.com/monksoul/Fur
// 开源协议：Apache-2.0（http://www.apache.org/licenses/LICENSE-2.0）
// -----------------------------------------------------------------------------

using Fur.DependencyInjection;
using Mapster;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Fur.DatabaseAccessor
{
    /// <summary>
    /// Sql 拓展类
    /// </summary>
    [NonBeScan]
    public static class SqlExtensions
    {
        /// <summary>
        /// 切换数据库上下文
        /// </summary>
        /// <typeparam name="TDbContextLocator">数据库上下文定位器</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <returns></returns>
        public static string Change<TDbContextLocator>(this string sql)
            where TDbContextLocator : class, IDbContextLocator
        {
            return $"{typeof(TDbContextLocator).FullName}{dbContextLocatorSqlSplit}{sql}";
        }

        /// <summary>
        /// Sql 查询返回 DataTable
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlQuery(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReader(sql, parameters);
        }

        /// <summary>
        /// Sql 查询返回 DataTable
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlQuery(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReader(sql, model.ToSqlParameters());
        }

        /// <summary>
        /// Sql 查询返回 DataTable
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlQueryAsync(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, parameters);
        }

        /// <summary>
        /// Sql 查询返回 DataTable
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlQueryAsync(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sql 查询返回 DataTable
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlQueryAsync(this string sql, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T></returns>
        public static List<T> SqlQuery<T>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReader(sql, parameters).ToList<T>();
        }

        /// <summary>
        /// Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>List<T></returns>
        public static List<T> SqlQuery<T>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteReader(sql, model.ToSqlParameters()).ToList<T>();
        }

        /// <summary>
        /// Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlQueryAsync<T>(this string sql, params object[] parameters)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, parameters);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlQueryAsync<T>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlQueryAsync<T>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref sql).ExecuteReaderAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        /// <summary>
        ///  Sql 查询返回 DataSet
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataSet</returns>
        public static DataSet SqlQueryMulti(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters);
        }

        /// <summary>
        ///  Sql 查询返回 DataSet
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>DataSet</returns>
        public static DataSet SqlQueryMulti(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters());
        }

        /// <summary>
        ///  Sql 查询返回 DataSet
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<DataSet></returns>
        public static Task<DataSet> SqlQueryMultiAsync(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
        }

        /// <summary>
        ///  Sql 查询返回 DataSet
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataSet></returns>
        public static Task<DataSet> SqlQueryMultiAsync(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///  Sql 查询返回 DataSet
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataSet></returns>
        public static Task<DataSet> SqlQueryMultiAsync(this string sql, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
        }

        /// <summary>
        ///  Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T1></returns>
        public static List<T1> SqlQueryMulti<T1>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2) SqlQueryMulti<T1, T2>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3) SqlQueryMulti<T1, T2, T3>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4) SqlQueryMulti<T1, T2, T3, T4>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5) SqlQueryMulti<T1, T2, T3, T4, T5>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6) SqlQueryMulti<T1, T2, T3, T4, T5, T6>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7) SqlQueryMulti<T1, T2, T3, T4, T5, T6, T7>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8) SqlQueryMulti<T1, T2, T3, T4, T5, T6, T7, T8>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, parameters).ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>List<T1></returns>
        public static List<T1> SqlQueryMulti<T1>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2) SqlQueryMulti<T1, T2>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3) SqlQueryMulti<T1, T2, T3>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4) SqlQueryMulti<T1, T2, T3, T4>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5) SqlQueryMulti<T1, T2, T3, T4, T5>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6) SqlQueryMulti<T1, T2, T3, T4, T5, T6>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7) SqlQueryMulti<T1, T2, T3, T4, T5, T6, T7>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8) SqlQueryMulti<T1, T2, T3, T4, T5, T6, T7, T8>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).DataAdapterFill(sql, model.ToSqlParameters()).ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<List<T1>></returns>
        public static async Task<List<T1>> SqlQueryMultiAsync<T1>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1>();
        }

        /// <summary>
        ///  Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T1>></returns>
        public static async Task<List<T1>> SqlQueryMultiAsync<T1>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlQueryMultiAsync<T1, T2>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlQueryMultiAsync<T1, T2>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlQueryMultiAsync<T1, T2, T3>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlQueryMultiAsync<T1, T2, T3>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlQueryMultiAsync<T1, T2, T3, T4>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlQueryMultiAsync<T1, T2, T3, T4>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlQueryMultiAsync<T1, T2, T3, T4, T5>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlQueryMultiAsync<T1, T2, T3, T4, T5>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string sql, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  Sql 查询返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>List<T1></returns>
        public static async Task<List<T1>> SqlQueryMultiAsync<T1>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlQueryMultiAsync<T1, T2>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlQueryMultiAsync<T1, T2, T3>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlQueryMultiAsync<T1, T2, T3, T4>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlQueryMultiAsync<T1, T2, T3, T4, T5>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// Sql 查询返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref sql).DataAdapterFillAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        /// 执行存储过程返回 DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlProcedureQuery(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReader(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlProcedureQuery(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReader(procName, model.ToSqlParameters(), CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataTable</returns>
        public static Task<DataTable> SqlProcedureQueryAsync(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>DataTable</returns>
        public static Task<DataTable> SqlProcedureQueryAsync(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程返回 DataTable
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>DataTable</returns>
        public static Task<DataTable> SqlProcedureQueryAsync(this string procName, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程返回 List 集合
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T></returns>
        public static List<T> SqlProcedureQuery<T>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReader(procName, parameters, CommandType.StoredProcedure).ToList<T>();
        }

        /// <summary>
        /// 执行存储过程返回 List 集合
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>List<T></returns>
        public static List<T> SqlProcedureQuery<T>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteReader(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T>();
        }

        /// <summary>
        /// 执行存储过程返回 List 集合
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T></returns>
        public static async Task<List<T>> SqlProcedureQueryAsync<T>(this string procName, params object[] parameters)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, parameters, CommandType.StoredProcedure);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 执行存储过程返回 List 集合
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>List<T></returns>
        public static async Task<List<T>> SqlProcedureQueryAsync<T>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 执行存储过程返回 List 集合
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>List<T></returns>
        public static async Task<List<T>> SqlProcedureQueryAsync<T>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataTable = await GetSqlRepositoryDatabase(ref procName).ExecuteReaderAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 执行存储过程返回 DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataSet</returns>
        public static DataSet SqlProcedureQueryMulti(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>DataSet</returns>
        public static DataSet SqlProcedureQueryMulti(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataSet</returns>
        public static Task<DataSet> SqlProcedureQueryMultiAsync(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>DataSet</returns>
        public static Task<DataSet> SqlProcedureQueryMultiAsync(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程返回 DataSet
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>DataSet</returns>
        public static Task<DataSet> SqlProcedureQueryMultiAsync(this string procName, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        ///  执行存储过程返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T1></returns>
        public static List<T1> SqlProcedureQueryMulti<T1>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2) SqlProcedureQueryMulti<T1, T2>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3) SqlProcedureQueryMulti<T1, T2, T3>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4) SqlProcedureQueryMulti<T1, T2, T3, T4>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5) SqlProcedureQueryMulti<T1, T2, T3, T4, T5>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6, T7>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6, T7, T8>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  执行存储过程返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>List<T1></returns>
        public static List<T1> SqlProcedureQueryMulti<T1>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2) SqlProcedureQueryMulti<T1, T2>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3) SqlProcedureQueryMulti<T1, T2, T3>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4) SqlProcedureQueryMulti<T1, T2, T3, T4>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5) SqlProcedureQueryMulti<T1, T2, T3, T4, T5>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6, T7>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>元组类型</returns>
        public static (List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8) SqlProcedureQueryMulti<T1, T2, T3, T4, T5, T6, T7, T8>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, model.ToSqlParameters(), CommandType.StoredProcedure).ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  执行存储过程返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<List<T1>></returns>
        public static async Task<List<T1>> SqlProcedureQueryMultiAsync<T1>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1>();
        }

        /// <summary>
        ///  执行存储过程返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T1>></returns>
        public static async Task<List<T1>> SqlProcedureQueryMultiAsync<T1>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlProcedureQueryMultiAsync<T1, T2>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlProcedureQueryMultiAsync<T1, T2>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlProcedureQueryMultiAsync<T1, T2, T3>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlProcedureQueryMultiAsync<T1, T2, T3>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string procName, params object[] parameters)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        ///  执行存储过程返回 List 集合
        /// </summary>
        /// <typeparam name="T1">返回类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>List<T1></returns>
        public static async Task<List<T1>> SqlProcedureQueryMultiAsync<T1>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2)> SqlProcedureQueryMultiAsync<T1, T2>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3)> SqlProcedureQueryMultiAsync<T1, T2, T3>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7>();
        }

        /// <summary>
        /// 执行存储过程返回 元组 集合
        /// </summary>
        /// <typeparam name="T1">元组元素类型</typeparam>
        /// <typeparam name="T2">元组元素类型</typeparam>
        /// <typeparam name="T3">元组元素类型</typeparam>
        /// <typeparam name="T4">元组元素类型</typeparam>
        /// <typeparam name="T5">元组元素类型</typeparam>
        /// <typeparam name="T6">元组元素类型</typeparam>
        /// <typeparam name="T7">元组元素类型</typeparam>
        /// <typeparam name="T8">元组元素类型</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>元组类型</returns>
        public static async Task<(List<T1> list1, List<T2> list2, List<T3> list3, List<T4> list4, List<T5> list5, List<T6> list6, List<T7> list7, List<T8> list8)> SqlProcedureQueryMultiAsync<T1, T2, T3, T4, T5, T6, T7, T8>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var dataset = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return dataset.ToList<T1, T2, T3, T4, T5, T6, T7, T8>();
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static object SqlProcedureScalar(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalar(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>object</returns>
        public static object SqlProcedureScalar(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalar(procName, model.ToSqlParameters(), CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static Task<object> SqlProcedureScalarAsync(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>object</returns>
        public static Task<object> SqlProcedureScalarAsync(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>object</returns>
        public static Task<object> SqlProcedureScalarAsync(this string procName, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static TResult SqlProcedureScalar<TResult>(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalar(procName, parameters, CommandType.StoredProcedure).Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>TResult</returns>
        public static TResult SqlProcedureScalar<TResult>(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteScalar(procName, model.ToSqlParameters(), CommandType.StoredProcedure).Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlProcedureScalarAsync<TResult>(this string procName, params object[] parameters)
        {
            var result = await GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, parameters, CommandType.StoredProcedure);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlProcedureScalarAsync<TResult>(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            var result = await GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程返回 单行单列
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlProcedureScalarAsync<TResult>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var result = await GetSqlRepositoryDatabase(ref procName).ExecuteScalarAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程无数据返回
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>int</returns>
        public static int SqlProcedureNonQuery(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteNonQuery(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程无数据返回
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <returns>int</returns>
        public static int SqlProcedureNonQuery(this string procName, object model)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteNonQuery(procName, model.ToSqlParameters(), CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程无数据返回
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>int</returns>
        public static Task<int> SqlProcedureNonQueryAsync(this string procName, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteNonQueryAsync(procName, parameters, CommandType.StoredProcedure);
        }

        /// <summary>
        /// 执行存储过程无数据返回
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>int</returns>
        public static Task<int> SqlProcedureNonQueryAsync(this string procName, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteNonQueryAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行存储过程无数据返回
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>int</returns>
        public static Task<int> SqlProcedureNonQueryAsync(this string procName, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref procName).ExecuteNonQueryAsync(procName, model.ToSqlParameters(), CommandType.StoredProcedure, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行 Sql 无数据返回
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>int</returns>
        public static int SqlNonQuery(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteNonQuery(sql, parameters);
        }

        /// <summary>
        /// 执行 Sql 无数据返回
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>int</returns>
        public static int SqlNonQuery(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteNonQuery(sql, model.ToSqlParameters());
        }

        /// <summary>
        /// 执行 Sql 无数据返回
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>int</returns>
        public static Task<int> SqlNonQueryAsync(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteNonQueryAsync(sql, parameters);
        }

        /// <summary>
        /// 执行 Sql 无数据返回
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>int</returns>
        public static Task<int> SqlNonQueryAsync(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteNonQueryAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行 Sql 无数据返回
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>int</returns>
        public static Task<int> SqlNonQueryAsync(this string sql, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteNonQueryAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static object SqlScalar(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalar(sql, parameters);
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>object</returns>
        public static object SqlScalar(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalar(sql, model.ToSqlParameters());
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static Task<object> SqlScalarAsync(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, parameters);
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>object</returns>
        public static Task<object> SqlScalarAsync(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>object</returns>
        public static Task<object> SqlScalarAsync(this string sql, object model, CancellationToken cancellationToken = default)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static TResult SqlScalar<TResult>(this string sql, params object[] parameters)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalar(sql, parameters).Adapt<TResult>();
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <returns>TResult</returns>
        public static TResult SqlScalar<TResult>(this string sql, object model)
        {
            return GetSqlRepositoryDatabase(ref sql).ExecuteScalar(sql, model.ToSqlParameters()).Adapt<TResult>();
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlScalarAsync<TResult>(this string sql, params object[] parameters)
        {
            var result = await GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, parameters);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlScalarAsync<TResult>(this string sql, object[] parameters, CancellationToken cancellationToken = default)
        {
            var result = await GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行 Sql 返回 单行单列
        /// </summary>
        /// <param name="sql">sql 语句</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlScalarAsync<TResult>(this string sql, object model, CancellationToken cancellationToken = default)
        {
            var result = await GetSqlRepositoryDatabase(ref sql).ExecuteScalarAsync(sql, model.ToSqlParameters(), cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>ProcedureOutput</returns>
        public static ProcedureOutputResult SqlProcedureOutput(this string procName, SqlParameter[] parameters)
        {
            parameters ??= Array.Empty<SqlParameter>();

            // 执行存储过程
            var dataSet = GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>ProcedureOutput</returns>
        public static async Task<ProcedureOutputResult> SqlProcedureOutputAsync(this string procName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            parameters ??= Array.Empty<SqlParameter>();

            // 执行存储过程
            var dataSet = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">命令模型</param>
        /// <returns>ProcedureOutput</returns>
        public static ProcedureOutputResult SqlProcedureOutput(this string procName, object model)
        {
            var parameters = model.ToSqlParameters();

            // 执行存储过程
            var dataSet = GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">命令模型</param>
        /// <returns>ProcedureOutput</returns>
        public static async Task<ProcedureOutputResult> SqlProcedureOutputAsync(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var parameters = model.ToSqlParameters();

            // 执行存储过程
            var dataSet = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <typeparam name="TResult">数据集结果</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>ProcedureOutput</returns>
        public static ProcedureOutputResult<TResult> SqlProcedureOutput<TResult>(this string procName, SqlParameter[] parameters)
        {
            parameters ??= Array.Empty<SqlParameter>();

            // 执行存储过程
            var dataSet = GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput<TResult>(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <typeparam name="TResult">数据集结果</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>ProcedureOutput</returns>
        public static async Task<ProcedureOutputResult<TResult>> SqlProcedureOutputAsync<TResult>(this string procName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            parameters ??= Array.Empty<SqlParameter>();

            // 执行存储过程
            var dataSet = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput<TResult>(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <typeparam name="TResult">数据集结果</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">命令模型</param>
        /// <returns>ProcedureOutput</returns>
        public static ProcedureOutputResult<TResult> SqlProcedureOutput<TResult>(this string procName, object model)
        {
            var parameters = model.ToSqlParameters();

            // 执行存储过程
            var dataSet = GetSqlRepositoryDatabase(ref procName).DataAdapterFill(procName, parameters, CommandType.StoredProcedure);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput<TResult>(parameters, dataSet);
        }

        /// <summary>
        /// 执行存储过程返回OUPUT、RETURN、结果集
        /// </summary>
        /// <typeparam name="TResult">数据集结果</typeparam>
        /// <param name="procName">存储过程名</param>
        /// <param name="model">命令模型</param>
        /// <returns>ProcedureOutput</returns>
        public static async Task<ProcedureOutputResult<TResult>> SqlProcedureOutputAsync<TResult>(this string procName, object model, CancellationToken cancellationToken = default)
        {
            var parameters = model.ToSqlParameters();

            // 执行存储过程
            var dataSet = await GetSqlRepositoryDatabase(ref procName).DataAdapterFillAsync(procName, parameters, CommandType.StoredProcedure, cancellationToken: cancellationToken);

            // 包装结果集
            return DbHelpers.WrapperProcedureOutput<TResult>(parameters, dataSet);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static object SqlFunctionScalar(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            return database.ExecuteScalar(sql, parameters);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>object</returns>
        public static object SqlFunctionScalar(this string funcName, object model)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, model);
            return database.ExecuteScalar(sql, parameters);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static Task<object> SqlFunctionScalarAsync(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            return database.ExecuteScalarAsync(sql, parameters);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>object</returns>
        public static Task<object> SqlFunctionScalarAsync(this string funcName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            return database.ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>object</returns>
        public static Task<object> SqlFunctionScalarAsync(this string funcName, object model, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, model);
            return database.ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static TResult SqlFunctionScalar<TResult>(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            return database.ExecuteScalar(sql, parameters).Adapt<TResult>();
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>TResult</returns>
        public static TResult SqlFunctionScalar<TResult>(this string funcName, object model)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, model);
            return database.ExecuteScalar(sql, parameters).Adapt<TResult>();
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlFunctionScalarAsync<TResult>(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            var result = await database.ExecuteScalarAsync(sql, parameters);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>TResult</returns>
        public static async Task<TResult> SqlFunctionScalarAsync<TResult>(this string funcName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, parameters);
            var result = await database.ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行标量函数返回 单行单列
        /// </summary>
        /// <typeparam name="TResult">返回值类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>object</returns>
        public static async Task<TResult> SqlFunctionScalarAsync<TResult>(this string funcName, object model, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Scalar, funcName, model);
            var result = await database.ExecuteScalarAsync(sql, parameters, cancellationToken: cancellationToken);
            return result.Adapt<TResult>();
        }

        /// <summary>
        /// 执行表值函数返回 DataTable
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlFunctionQuery(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            return database.ExecuteReader(sql, parameters);
        }

        /// <summary>
        /// 执行表值函数返回 DataTable
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>DataTable</returns>
        public static DataTable SqlFunctionQuery(this string funcName, object model)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, model);
            return database.ExecuteReader(sql, parameters);
        }

        /// <summary>
        /// 执行表值函数返回 DataTable
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlFunctionQueryAsync(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            return database.ExecuteReaderAsync(sql, parameters);
        }

        /// <summary>
        /// 执行表值函数返回 DataTable
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlFunctionQueryAsync(this string funcName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            return database.ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行表值函数返回 DataTable
        /// </summary>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<DataTable></returns>
        public static Task<DataTable> SqlFunctionQueryAsync(this string funcName, object model, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, model);
            return database.ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
        }

        /// <summary>
        /// 执行表值函数返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>List<T></returns>
        public static List<T> SqlFunctionQuery<T>(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            return database.ExecuteReader(sql, parameters).ToList<T>();
        }

        /// <summary>
        /// 执行表值函数返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <returns>List<T></returns>
        public static List<T> SqlFunctionQuery<T>(this string funcName, object model)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, model);
            return database.ExecuteReader(sql, parameters).ToList<T>();
        }

        /// <summary>
        /// 执行表值函数返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlFunctionQueryAsync<T>(this string funcName, params SqlParameter[] parameters)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            var dataTable = await database.ExecuteReaderAsync(sql, parameters);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 执行表值函数返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="parameters">命令参数</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlFunctionQueryAsync<T>(this string funcName, SqlParameter[] parameters, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var sql = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, parameters);
            var dataTable = await database.ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        /// <summary>
        /// 执行表值函数返回 List 集合
        /// </summary>
        /// <typeparam name="T">返回类型</typeparam>
        /// <param name="funcName">函数名</param>
        /// <param name="model">参数模型</param>
        /// <param name="cancellationToken">异步取消令牌</param>
        /// <returns>Task<List<T>></returns>
        public static async Task<List<T>> SqlFunctionQueryAsync<T>(this string funcName, object model, CancellationToken cancellationToken = default)
        {
            var database = GetSqlRepositoryDatabase(ref funcName);
            var (sql, parameters) = DbHelpers.CombineFunctionSql(database.ProviderName, DbFunctionType.Table, funcName, model);
            var dataTable = await database.ExecuteReaderAsync(sql, parameters, cancellationToken: cancellationToken);
            return dataTable.ToList<T>();
        }

        // 分隔符
        private static readonly string dbContextLocatorSqlSplit;

        /// <summary>
        /// 静态构造函数
        /// </summary>
        static SqlExtensions()
        {
            dbContextLocatorSqlSplit = "-=>";
        }

        /// <summary>
        /// 获取 Sql 仓储数据库操作对象
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        private static DatabaseFacade GetSqlRepositoryDatabase(ref string sql)
        {
            // 分隔符
            Type dbContextLocator;
            if (sql.Contains(dbContextLocatorSqlSplit))
            {
                dbContextLocator = Type.GetType(sql.Split(dbContextLocatorSqlSplit).First());
                sql = sql[(sql.IndexOf(dbContextLocatorSqlSplit) + dbContextLocatorSqlSplit.Length)..];
            }
            else
            {
                dbContextLocator = typeof(DbContextLocator);
            }

            // 创建Sql仓储
            var sqlRepositoryType = typeof(ISqlRepository<>).MakeGenericType(dbContextLocator);
            var sqlRepository = App.RequestServiceProvider.GetService(sqlRepositoryType);

            return sqlRepositoryType.GetProperty(nameof(ISqlRepository.Database)).GetValue(sqlRepository) as DatabaseFacade;
        }
    }
}