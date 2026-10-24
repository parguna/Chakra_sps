using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DI.Chakra.Infrastructure.Enums
{
    public enum DiscoveryObjectType : int
    {
        None = 0,
        User ,

        Group,

        Role,

        Certifier,

        Profile,

        Position,

        Parameter,

        TCode,
        UserGroup,
        UserRole,
        RoleTCode,
        Status,
        Retry,
        Connect,
        Domain,
        Folder,
        Printer,
        Computer,
        Voulme,
        ADComputerGroup,
        OrganizationalUnit,
        LastLogin,
        ADUser,
        Server,
        Authorization,
        Field,
        SAP,
        MappingTable,
        Propagate,
        Process,
        InActive,
        Email,
        UpdateManager,
        Company,
        Center,
        PersonnelArea,
        PersonnelSubArea,
        BusinessRole,Section,Badge, AccessLevel, UserAccessLevel, License, RetentionPolicy, SharingPolicy, ResponseGroup,
        CriticalTCodeReport, ConflictingRoleReport

    }
}
