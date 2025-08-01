﻿using Core.Models;

namespace Infrastructure.Integrations.Interfaces;

public interface ICounterpartyDataFactory
{
    IPartyInfoStrategy GetDataStrategy(DocumentType type);
}