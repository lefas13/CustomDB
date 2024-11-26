using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using CustomMVC.Models;

namespace CustomMVC.Data;

public partial class CustomContext : DbContext
{
    public CustomContext()
    {
    }

    public CustomContext(DbContextOptions<CustomContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Agent> Agents { get; set; }

    public virtual DbSet<Fee> Fees { get; set; }

    public virtual DbSet<Good> Goods { get; set; }

    public virtual DbSet<GoodType> GoodTypes { get; set; }

    public virtual DbSet<Warehouse> Warehouses { get; set; }

    // Get an agent by ID
    public Agent GetAgentById(int id)
    {
        return Agents.Find(id);
    }

    // Add a new agent
    public void AddAgent(Agent agent)
    {
        Agents.Add(agent);
        SaveChanges();
    }

    // Update an existing agent
    public void UpdateAgent(Agent agent)
    {
        Agents.Update(agent);
        SaveChanges();
    }

    // Delete an agent by ID
    public void DeleteAgent(int id)
    {
        Agent agent = GetAgentById(id);
        if (agent != null)
        {
            Agents.Remove(agent);
            SaveChanges();
        }
    }

    // Check if an agent exists
    public bool AgentExists(int id)
    {
        return Agents.Any(e => e.AgentId == id);
    }
}

