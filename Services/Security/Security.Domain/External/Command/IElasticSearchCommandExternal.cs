﻿using Security.Domain.CQRS.External.Commands;
using Security.Domain.Entities;
using Security.Domain.Repositories.Command.Base;
using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Security.Domain.External.Command
{
    /// <summary>
    /// Interface elastic search command external
    /// </summary>
    public interface IElasticSearchCommandExternal : ISingleCommandRepository<RequestElasticSearchCommand>
    {
    }
}
