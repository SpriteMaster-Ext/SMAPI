using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using StardewModdingAPI.Toolkit.Framework.Clients.CompatibilityRepo;
using StardewModdingAPI.Toolkit.Framework.ModData;

namespace StardewModdingAPI.Toolkit.Framework.Clients.WebApi;

/// <summary>Extended metadata about a mod.</summary>
public class ModExtendedMetadataModel
{
    /*********
    ** Accessors
    *********/
    /****
    ** Mod info
    ****/
    /// <summary>The mod's unique ID. A mod may have multiple current IDs in rare cases (e.g. due to parallel releases or unofficial updates).</summary>
    public string[] ID { get; set; } = Array.Empty<string>();

    /// <summary>The mod's display name.</summary>
    public string? Name { get; set; }

    /// <summary>The mod ID on Nexus.</summary>
    public int? NexusID { get; set; }

    /// <summary>The mod ID in the Chucklefish mod repo.</summary>
    public int? ChucklefishID { get; set; }

    /// <summary>The mod ID in the CurseForge mod repo.</summary>
    public int? CurseForgeID { get; set; }

    /// <summary>The mod ID in the ModDrop mod repo.</summary>
    public int? ModDropID { get; set; }

    /// <summary>The GitHub repository in the form 'owner/repo'.</summary>
    public string? GitHubRepo { get; set; }

    /// <summary>The URL to a non-GitHub source repo.</summary>
    public string? CustomSourceUrl { get; set; }

    /// <summary>The custom mod page URL (if applicable).</summary>
    public string? CustomUrl { get; set; }

    /// <summary>The main version.</summary>
    public ModEntryVersionModel? Main { get; set; }

    /// <summary>The latest optional version, if newer than <see cref="Main"/>.</summary>
    public ModEntryVersionModel? Optional { get; set; }

    /// <summary>The latest unofficial version, if newer than <see cref="Main"/> and <see cref="Optional"/>.</summary>
    public ModEntryVersionModel? Unofficial { get; set; }

    /****
    ** Stable compatibility
    ****/
    /// <summary>The compatibility status.</summary>
    [JsonConverter(typeof(StringEnumConverter))]
    public ModCompatibilityStatus? CompatibilityStatus { get; set; }

    /// <summary>The human-readable summary of the compatibility status or workaround, without HTML formatting.</summary>
    public string? CompatibilitySummary { get; set; }

    /// <summary>The game or SMAPI version which broke this mod, if applicable.</summary>
    public string? BrokeIn { get; set; }

    /****
    ** Version mappings
    ****/
    /// <summary>A serialized change descriptor to apply to the local version during update checks (see <see cref="ChangeDescriptor"/>).</summary>
    public string? ChangeLocalVersions { get; set; }

    /// <summary>A serialized change descriptor to apply to the remote version during update checks (see <see cref="ChangeDescriptor"/>).</summary>
    public string? ChangeRemoteVersions { get; set; }

    /// <summary>A serialized change descriptor to apply to the update keys during update checks (see <see cref="ChangeDescriptor"/>).</summary>
    public string? ChangeUpdateKeys { get; set; }


    /*********
    ** Public methods
    *********/
    /// <summary>Construct an instance.</summary>
    public ModExtendedMetadataModel() { }

    /// <summary>Construct an instance.</summary>
    /// <param name="compatibility">The mod metadata from the mod compatibility list (if available).</param>
    /// <param name="db">The mod metadata from SMAPI's internal DB (if available).</param>
    /// <param name="main">The main version.</param>
    /// <param name="optional">The latest optional version, if newer than <paramref name="main"/>.</param>
    /// <param name="unofficial">The latest unofficial version, if newer than <paramref name="main"/> and <paramref name="optional"/>.</param>
    public ModExtendedMetadataModel(ModCompatibilityEntry? compatibility, ModDataRecord? db, ModEntryVersionModel? main, ModEntryVersionModel? optional, ModEntryVersionModel? unofficial)
    {
        // versions
        this.Main = main;
        this.Optional = optional;
        this.Unofficial = unofficial;

        // compatibility list data
        if (compatibility != null)
        {
            this.ID = compatibility.ID;
            this.Name = compatibility.Name.FirstOrDefault();
            this.NexusID = compatibility.NexusID;
            this.ChucklefishID = compatibility.ChucklefishID;
            this.CurseForgeID = compatibility.CurseForgeID;
            this.ModDropID = compatibility.ModDropID;
            this.GitHubRepo = compatibility.GitHubRepo;
            this.CustomSourceUrl = compatibility.CustomSourceUrl;
            this.CustomUrl = compatibility.CustomUrl;

            this.CompatibilityStatus = compatibility.Compatibility.Status;
            this.CompatibilitySummary = compatibility.Compatibility.Summary;
            this.BrokeIn = compatibility.Compatibility.BrokeIn;

            this.ChangeLocalVersions = compatibility.Overrides?.ChangeLocalVersions?.ToString();
            this.ChangeRemoteVersions = compatibility.Overrides?.ChangeRemoteVersions?.ToString();
            this.ChangeUpdateKeys = compatibility.Overrides?.ChangeUpdateKeys?.ToString();
        }

        // internal DB data
        if (db != null)
        {
            this.ID = this.ID.Union(db.FormerIDs).ToArray();
            this.Name ??= db.DisplayName;
        }
    }

    /// <summary>Get update keys based on the metadata.</summary>
    public IEnumerable<string> GetUpdateKeys()
    {
        if (this.NexusID.HasValue)
            yield return $"Nexus:{this.NexusID}";
        if (this.ChucklefishID.HasValue)
            yield return $"Chucklefish:{this.ChucklefishID}";
        if (this.GitHubRepo != null)
            yield return $"GitHub:{this.GitHubRepo}";
    }
}
