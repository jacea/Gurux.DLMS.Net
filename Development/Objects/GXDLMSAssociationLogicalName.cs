//
// --------------------------------------------------------------------------
//  Gurux Ltd
// 
//
//
// Filename:        $HeadURL$
//
// Version:         $Revision$,
//                  $Date$
//                  $Author$
//
// Copyright (c) Gurux Ltd
//
//---------------------------------------------------------------------------
//
//  DESCRIPTION
//
// This file is a part of Gurux Device Framework.
//
// Gurux Device Framework is Open Source software; you can redistribute it
// and/or modify it under the terms of the GNU General Public License 
// as published by the Free Software Foundation; version 2 of the License.
// Gurux Device Framework is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of 
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE. 
// See the GNU General Public License for more details.
//
// More information of Gurux products: http://www.gurux.org
//
// This code is licensed under the GNU General Public License v2. 
// Full text may be retrieved at http://www.gnu.org/licenses/gpl-2.0.txt
//---------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Gurux.DLMS.ManufacturerSettings;
using System.Xml.Serialization;
using Gurux.DLMS.Internal;

namespace Gurux.DLMS.Objects
{
    public enum AssociationStatus
    {
        NonAssociated = 0,
        AssociationPending = 1,
        Associated = 2
    }

    public class GXDLMSAssociationLogicalName : GXDLMSObject, IGXDLMSBase
    {
        /// <summary> 
        /// Constructor.
        /// </summary> 
        public GXDLMSAssociationLogicalName()
            : this("0.0.40.0.0.255")
        {
            ObjectList = new GXDLMSObjectCollection();
            ApplicationContextName = new GXApplicationContextName();
            XDLMSContextInfo = new GXxDLMSContextType();
            AuthenticationMechanismMame = new GXAuthenticationMechanismName();
        }

        /// <summary> 
        /// Constructor.
        /// </summary> 
        /// <param name="ln">Logican Name of the object.</param>
        public GXDLMSAssociationLogicalName(string ln)
            : base(ObjectType.AssociationLogicalName, ln, 0)
        {
            ObjectList = new GXDLMSObjectCollection();
            ApplicationContextName = new GXApplicationContextName();
            XDLMSContextInfo = new GXxDLMSContextType();
            AuthenticationMechanismMame = new GXAuthenticationMechanismName();
        }

        [XmlIgnore()]
        public GXDLMSObjectCollection ObjectList
        {
            get;
            internal set;
        }

        /// <summary>
        /// Contains the identifiers of the COSEM client APs within the physical devices hosting these APs, 
        /// which belong to the AA modelled by the �Association LN� object.
        /// </summary>
        [XmlIgnore()]
        public byte ClientSAP
        {
            get;
            set;
        }

        /// <summary>
        /// Contains the identifiers of the COSEM server (logical device) APs within the physical 
        /// devices hosting these APs, which belong to the AA modelled by the �Association LN� object.
        /// </summary>
        [XmlIgnore()]
        public UInt16 ServerSAP
        {
            get;
            set;
        }

        [XmlIgnore()]
        public GXApplicationContextName ApplicationContextName
        {
            get;
            private set;
        }

        [XmlIgnore()]
        public GXxDLMSContextType XDLMSContextInfo
        {
            get;
            internal set;
        }


        [XmlIgnore()]
        public GXAuthenticationMechanismName AuthenticationMechanismMame
        {
            get;
            internal set;
        }

        [XmlIgnore()]
        public byte[] Secret
        {
            get;
            set;
        }

        [XmlIgnore()]
        public AssociationStatus AssociationStatus
        {
            get;
            set;
        }

        [XmlIgnore()]
        public string SecuritySetupReference
        {
            get;
            set;
        }

        /// <inheritdoc cref="GXDLMSObject.GetValues"/>
        public override object[] GetValues()
        {
            return new object[] { LogicalName, ObjectList, ClientSAP + "/" + ServerSAP, ApplicationContextName,
            XDLMSContextInfo, AuthenticationMechanismMame, Secret, AssociationStatus, SecuritySetupReference};
        }

        #region IGXDLMSBase Members

        byte[][] IGXDLMSBase.Invoke(object sender, int index, Object parameters)
        {
            //Check reply_to_HLS_authentication
            if (index == 1)
            {
                GXDLMSServerBase s = sender as GXDLMSServerBase;
                if (s == null)
                {
                    throw new ArgumentException("sender");
                }
                GXDLMS b = s.m_Base;
                //Get server Challenge.
                List<byte> challenge = null;
                List<byte> CtoS = null;
                //Find shared secret
                foreach (GXAuthentication it in s.Authentications)
                {
                    if (it.Type == b.Authentication)
                    {
                        CtoS = new List<byte>(it.SharedSecret);
                        challenge = new List<byte>(it.SharedSecret);
                        challenge.AddRange(b.StoCChallenge);
                        break;
                    }
                }
                byte[] serverChallenge = GXDLMS.Chipher(b.Authentication, challenge.ToArray());
                byte[] clientChallenge = (byte[])parameters;
                int pos = 0;
                if (GXCommon.Compare(serverChallenge, ref pos, clientChallenge))
                {
                    CtoS.AddRange(b.CtoSChallenge);
                    return s.Acknowledge(Command.MethodResponse, 0, GXDLMS.Chipher(b.Authentication, CtoS.ToArray()), DataType.OctetString);
                }
                else
                {
                    //Return error.
                    return s.ServerReportError(Command.MethodRequest, 5);
                }
            }
            else
            {
                throw new ArgumentException("Invoke failed. Invalid attribute index.");
            }
        }

        int[] IGXDLMSBase.GetAttributeIndexToRead()
        {
            List<int> attributes = new List<int>();
            //LN is static and read only once.
            if (string.IsNullOrEmpty(LogicalName))
            {
                attributes.Add(1);
            }

            //ObjectList is static and read only once.
            if (!base.IsRead(2))
            {
                attributes.Add(2);
            }

            //associated_partners_id is static and read only once.
            if (!base.IsRead(3))
            {
                attributes.Add(3);
            }
            //Application Context Name is static and read only once.
            if (!base.IsRead(4))
            {
                attributes.Add(4);
            }

            //xDLMS Context Info
            if (!base.IsRead(5))
            {
                attributes.Add(5);
            }

            // Authentication Mechanism Name
            if (!base.IsRead(6))
            {
                attributes.Add(6);
            }

            // Secret
            if (!base.IsRead(7))
            {
                attributes.Add(7);
            }
            // Association Status
            if (!base.IsRead(8))
            {
                attributes.Add(8);
            }
            //Security Setup Reference is from version 1.
            if (Version > 0 && !base.IsRead(9))
            {
                attributes.Add(9);
            }
            return attributes.ToArray();
        }

        int IGXDLMSBase.GetAttributeCount()
        {
            //Security Setup Reference is from version 1.
            if (Version > 0)
                return 9;
            return 8;
        }

        int IGXDLMSBase.GetMethodCount()
        {
            return 4;
        }

        /// <summary>
        /// Returns Association View.    
        /// </summary>     
        private byte[] GetObjects()
        {
            List<byte> stream = new List<byte>();
            stream.Add((byte)DataType.Array);
            bool lnExists = ObjectList.FindByLN(ObjectType.AssociationLogicalName, this.LogicalName) != null;
            //Add count        
            int cnt = ObjectList.Count();
            if (!lnExists)
            {
                ++cnt;
            }
            GXCommon.SetObjectCount(cnt, stream);
            foreach (GXDLMSObject it in ObjectList)
            {
                stream.Add((byte)DataType.Structure);
                stream.Add((byte)4); //Count
                GXCommon.SetData(stream, DataType.UInt16, it.ObjectType); //ClassID
                GXCommon.SetData(stream, DataType.UInt8, it.Version); //Version
                GXCommon.SetData(stream, DataType.OctetString, it.LogicalName); //LN
                GetAccessRights(it, stream); //Access rights.
            }
            if (!lnExists)
            {
                stream.Add((byte)DataType.Structure);
                stream.Add((byte)4); //Count
                GXCommon.SetData(stream, DataType.UInt16, this.ObjectType); //ClassID
                GXCommon.SetData(stream, DataType.UInt8, this.Version); //Version
                GXCommon.SetData(stream, DataType.OctetString, this.LogicalName); //LN
                GetAccessRights(this, stream); //Access rights.
            }
            return stream.ToArray();
        }

        private void GetAccessRights(GXDLMSObject item, List<byte> data)
        {
            data.Add((byte)DataType.Structure);
            data.Add((byte)2);
            data.Add((byte)DataType.Array);
            GXAttributeCollection attributes = item.Attributes;
            int cnt = (item as IGXDLMSBase).GetAttributeCount();
            data.Add((byte)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                GXDLMSAttributeSettings att = attributes.Find(pos + 1);
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add((byte)3);
                GXCommon.SetData(data, DataType.Int8, pos + 1);
                //If attribute is not set return read only.
                if (att == null)
                {
                    GXCommon.SetData(data, DataType.Enum, AccessMode.Read);
                }
                else
                {
                    GXCommon.SetData(data, DataType.Enum, att.Access);
                }
                GXCommon.SetData(data, DataType.None, null);
            }
            data.Add((byte)DataType.Array);
            attributes = item.MethodAttributes;
            cnt = (item as IGXDLMSBase).GetMethodCount();
            data.Add((byte)cnt);
            for (int pos = 0; pos != cnt; ++pos)
            {
                GXDLMSAttributeSettings att = attributes.Find(pos + 1);
                data.Add((byte)DataType.Structure); //attribute_access_item
                data.Add((byte)2);
                GXCommon.SetData(data, DataType.Int8, pos + 1);
                //If method attribute is not set return no access.
                if (att == null)
                {
                    GXCommon.SetData(data, DataType.Enum, MethodAccessMode.NoAccess);
                }
                else
                {
                    GXCommon.SetData(data, DataType.Enum, att.MethodAccess);
                }
            }
        }

        void UpdateAccessRights(GXDLMSObject obj, Object[] buff)
        {
            foreach (Object[] attributeAccess in (Object[])buff[0])
            {
                int id = Convert.ToInt32(attributeAccess[0]);
                int mode = Convert.ToInt32(attributeAccess[1]);
                obj.SetAccess(id, (AccessMode)mode);
            }
            foreach (Object[] methodAccess in (Object[])buff[1])
            {
                int id = Convert.ToInt32(methodAccess[0]);
                int tmp;
                //If version is 0.
                if (methodAccess[1] is Boolean)
                {
                    tmp = ((Boolean)methodAccess[1]) ? 1 : 0;
                }
                else//If version is 1.
                {
                    tmp = Convert.ToInt32(methodAccess[1]);
                }
                obj.SetMethodAccess(id, (MethodAccessMode)tmp);
            }
        }

        override public DataType GetDataType(int index)
        {
            if (index == 1)
            {
                return DataType.OctetString;
            }
            if (index == 2)
            {
                return DataType.Array;
            }
            if (index == 3)
            {
                return DataType.Structure;
            }
            if (index == 4)
            {
                return DataType.Structure;
            }
            if (index == 5)
            {
                return DataType.Structure;
            }
            if (index == 6)
            {
                return DataType.Structure;
            }
            if (index == 7)
            {
                return DataType.OctetString;
            }
            if (index == 8)
            {
                return DataType.Enum;
            }
            if (index == 9)
            {
                return DataType.OctetString;
            }
            throw new ArgumentException("GetDataType failed. Invalid attribute index.");
        }

        object IGXDLMSBase.GetValue(int index, int selector, object parameters)
        {
            if (index == 1)
            {
                return GXDLMSObject.GetLogicalName(this.LogicalName);
            }
            if (index == 2)
            {
                return GetObjects();
            }
            if (index == 3)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Array);
                //Add count            
                data.Add(2);
                data.Add((byte)DataType.UInt8);
                data.Add(ClientSAP);
                data.Add((byte)DataType.UInt16);
                GXCommon.SetUInt16(ServerSAP, data);
                return data.ToArray();
            }
            if (index == 4)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Structure);
                //Add count            
                data.Add(0x7);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.JointIsoCtt);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.Country);
                GXCommon.SetData(data, DataType.UInt16, ApplicationContextName.CountryName);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.IdentifiedOrganization);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.DlmsUA);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.ApplicationContext);
                GXCommon.SetData(data, DataType.UInt8, ApplicationContextName.ContextId);
                return data.ToArray();               
            }
            if (index == 5)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Structure);
                data.Add(6);
                GXCommon.SetData(data, DataType.BitString, XDLMSContextInfo.Conformance);
                GXCommon.SetData(data, DataType.UInt16, XDLMSContextInfo.MaxReceivePduSize);
                GXCommon.SetData(data, DataType.UInt16, XDLMSContextInfo.MaxSendPpuSize);
                GXCommon.SetData(data, DataType.UInt8, XDLMSContextInfo.DlmsVersionNumber);
                GXCommon.SetData(data, DataType.Int8, XDLMSContextInfo.QualityOfService);
                GXCommon.SetData(data, DataType.OctetString, XDLMSContextInfo.CypheringInfo);
                return data.ToArray();     
            }
            if (index == 6)
            {
                List<byte> data = new List<byte>();
                data.Add((byte)DataType.Structure);
                //Add count            
                data.Add(0x7);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.JointIsoCtt);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.Country);
                GXCommon.SetData(data, DataType.UInt16, AuthenticationMechanismMame.CountryName);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.IdentifiedOrganization);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.DlmsUA);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.AuthenticationMechanismName);
                GXCommon.SetData(data, DataType.UInt8, AuthenticationMechanismMame.MechanismId);
                return data.ToArray();     
            }
            if (index == 7)
            {
                return Secret;
            }
            if (index == 8)
            {
                return AssociationStatus;
            }
            if (index == 9)
            {
                if (SecuritySetupReference == null)
                {
                    return null;
                }
                return ASCIIEncoding.ASCII.GetBytes(SecuritySetupReference);
            }
            throw new ArgumentException("GetValue failed. Invalid attribute index.");
        }

        void IGXDLMSBase.SetValue(int index, object value)
        {
            if (index == 1)
            {
                if (value is string)
                {
                    LogicalName = value.ToString();
                }
                else
                {
                    LogicalName = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
                }
            }
            else if (index == 2)
            {
                ObjectList.Clear();
                if (value != null)
                {
                    foreach (Object[] item in (Object[])value)
                    {
                        ObjectType type = (ObjectType)Convert.ToInt32(item[0]);
                        int version = Convert.ToInt32(item[1]);
                        String ln = GXDLMSObject.toLogicalName((byte[])item[2]);
                        GXDLMSObject obj = Parent.FindByLN(type, ln);                           
                        if (obj == null)
                        {
                            obj = Gurux.DLMS.GXDLMSClient.CreateObject(type);
                            obj.LogicalName = ln;                            
                            obj.Version = version;                            
                        }
                        UpdateAccessRights(obj, (Object[])item[3]);
                        ObjectList.Add(obj);
                    }
                }
            }
            else if (index == 3)
            {
                ClientSAP = Convert.ToByte(((Object[])value)[0]);
                ServerSAP = Convert.ToUInt16(((Object[])value)[1]);
            }
            else if (index == 4)
            {                
                //Value of the object identifier encoded in BER
                if (value is byte[])
                {                    
                    int pos = -1;
                    byte[] arr = value as byte[];
                    if (arr[0] == 0x60)
                    {
                        ApplicationContextName.JointIsoCtt = 0;
                        ++pos;                        
                        ApplicationContextName.Country = 0;
                        ++pos;
                        ApplicationContextName.CountryName = 0;
                        ++pos;
                        ApplicationContextName.IdentifiedOrganization = arr[++pos];
                        ApplicationContextName.DlmsUA = arr[++pos];
                        ApplicationContextName.ApplicationContext = arr[++pos];
                        ApplicationContextName.ContextId = arr[++pos];
                    }
                    else
                    {
                        //Get Tag and Len.
                        if (arr[++pos] != (int)GXBer.IntegerTag && arr[++pos] != 7)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.JointIsoCtt = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.Country = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x12)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.CountryName = GXCommon.GetUInt16(arr, ref pos);
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.IdentifiedOrganization = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.DlmsUA = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.ApplicationContext = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        ApplicationContextName.ContextId = arr[++pos];
                    }
                }
                else
                {
                    Object[] arr = (Object[])value;
                    ApplicationContextName.JointIsoCtt = Convert.ToByte(arr[0]);
                    ApplicationContextName.Country = Convert.ToByte(arr[1]);
                    ApplicationContextName.CountryName = Convert.ToUInt16(arr[2]);
                    ApplicationContextName.IdentifiedOrganization = Convert.ToByte(arr[3]);
                    ApplicationContextName.DlmsUA = Convert.ToByte(arr[4]);
                    ApplicationContextName.ApplicationContext = Convert.ToByte(arr[5]);
                    ApplicationContextName.ContextId = Convert.ToByte(arr[6]);
                }
            }
            else if (index == 5)
            {
                Object[] arr = (Object[])value;
                XDLMSContextInfo.Conformance = arr[0].ToString();
                XDLMSContextInfo.MaxReceivePduSize = Convert.ToUInt16(arr[1]);
                XDLMSContextInfo.MaxSendPpuSize = Convert.ToUInt16(arr[2]);
                XDLMSContextInfo.DlmsVersionNumber = Convert.ToByte(arr[3]);
                XDLMSContextInfo.QualityOfService = Convert.ToSByte(arr[4]);
                XDLMSContextInfo.CypheringInfo = (byte[])arr[5];
            }
            else if (index == 6)
            {
                //Value of the object identifier encoded in BER
                if (value is byte[])
                {
                    int pos = -1;
                    byte[] arr = value as byte[];
                    if (arr[0] == 0x60)
                    {
                        AuthenticationMechanismMame.JointIsoCtt = 0;
                        ++pos;
                        AuthenticationMechanismMame.Country = 0;
                        ++pos;
                        AuthenticationMechanismMame.CountryName = 0;
                        ++pos;
                        AuthenticationMechanismMame.IdentifiedOrganization = arr[++pos];
                        AuthenticationMechanismMame.DlmsUA = arr[++pos];
                        AuthenticationMechanismMame.AuthenticationMechanismName = arr[++pos];
                        AuthenticationMechanismMame.MechanismId = (Authentication)arr[++pos];
                    }
                    else
                    {
                        //Get Tag and Len.
                        if (arr[++pos] != (int)GXBer.IntegerTag && arr[++pos] != 7)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.JointIsoCtt = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.Country = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x12)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.CountryName = GXCommon.GetUInt16(arr, ref pos);
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.IdentifiedOrganization = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.DlmsUA = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.AuthenticationMechanismName = arr[++pos];
                        //Get tag
                        if (arr[++pos] != 0x11)
                        {
                            throw new ArgumentOutOfRangeException();
                        }
                        AuthenticationMechanismMame.MechanismId = (Authentication) arr[++pos];
                    }
                }
                else
                {
                    Object[] arr = (Object[])value;
                    AuthenticationMechanismMame.JointIsoCtt = Convert.ToByte(arr[0]);
                    AuthenticationMechanismMame.Country = Convert.ToByte(arr[1]);
                    AuthenticationMechanismMame.CountryName = Convert.ToUInt16(arr[2]);
                    AuthenticationMechanismMame.IdentifiedOrganization = Convert.ToByte(arr[3]);
                    AuthenticationMechanismMame.DlmsUA = Convert.ToByte(arr[4]);
                    AuthenticationMechanismMame.AuthenticationMechanismName = Convert.ToByte(arr[5]);
                    AuthenticationMechanismMame.MechanismId = (Authentication) Convert.ToByte(arr[6]);
                }
            }
            else if (index == 7)
            {
                Secret = (byte[])value;
            }
            else if (index == 8)
            {
                if (value == null)
                {
                    AssociationStatus = AssociationStatus.NonAssociated;
                }
                else
                {
                    AssociationStatus = (AssociationStatus)Convert.ToInt32(value);
                }
            }
            else if (index == 9)
            {
                SecuritySetupReference = GXDLMSClient.ChangeType((byte[])value, DataType.OctetString).ToString();
            }
            else
            {
                throw new ArgumentException("SetValue failed. Invalid attribute index.");
            }
        }
        #endregion
    }
}
