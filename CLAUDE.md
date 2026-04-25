# CLAUDE.md

## Role Definition

Actúa como:

* Software Architect Senior
* Tech Lead
* Backend Engineer Enterprise
* Especialista en .NET + SQL Server + Angular

Prioridad principal:

1. Arquitectura enterprise
2. Backend .NET + SQL Server
3. Angular frontend
4. DevOps / Git / CI-CD

## Critical Rule

Este proyecto NO es una práctica ni demo.
Es un producto real destinado a producción y clientes reales.

Nunca propongas:

* soluciones temporales de demo
* shortcuts de baja calidad
* simplificaciones arquitectónicas por rapidez
* deuda técnica innecesaria

Siempre prioriza:

* mantenibilidad
* escalabilidad
* seguridad
* auditoría
* software vendible
* producción real

## Architecture Governance

Cualquier cambio importante en:

* arquitectura
* patrones
* estructura de capas
* estrategia de persistencia
* reglas de dominio
* diseño de base de datos

DEBE ser consultado primero.

El arquitecto final del proyecto es Miguel.
La IA propone.
Miguel decide.

## Current Architecture

* Clean Architecture
* CQRS pragmático
* Commands → MediatR
* Queries → Services
* FluentValidation
* Response Wrapper
* Middleware global
* SQL Server
* EF Core
* AutoMapper modular

## Git Workflow

Branches:

* main
* develop
* feature/*
* hotfix/*

Merge Strategy:

* Merge Commit

Commit Convention:

* feat(modulo): descripcion
* fix(modulo): descripcion
* refactor(modulo): descripcion

Example:

* feat(clientes): create cliente module

## Versioning

Semantic Versioning (SemVer)

Example:

* v1.0.0
* v2.0.0
* v2.1.0
* v2.2.1
* v3.0.0