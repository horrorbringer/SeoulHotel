using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace SeoulHotel.Coonnection
{
    public class DB
    {
        private SqlConnection conn;

        public DB()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDb"].ConnectionString;
            conn = new SqlConnection(connectionString);
        }

        public SqlConnection GetConnection()
        {
            return conn;
        }

        public void Open()
        {
            if (conn.State == System.Data.ConnectionState.Closed)
            {
                conn.Open();
            }
        }

        public void Close()
        {
            if (conn.State == System.Data.ConnectionState.Open)
            {
                conn.Close();
            }
        }

        public string insertItems()
        {
            return @"
                    INSERT INTO Items(
                    GUID, UserID, ItemTypeID, AreaID, Title, Capacity,
                    NumberOfBeds, NumberOfBedrooms, NumberOfBathrooms,
                    ExactAddress, ApproximateAddress, Description,
                    HostRules, MinimumNights, MaximumNights
                )
                OUTPUT INSERTED.ID
                VALUES (
                    @guid, @user_id, @item_type_id, @area_id, @title, @capacity,
                    @number_of_beds, @number_of_bedrooms, @number_of_bathrooms,
                    @exact_address, @approximate_address, @description,
                    @host_rule, @mimimum_night, @maximum_night
                );";
        }
        public string updateItem()
        {
            return @"UPDATE Items 
                    SET GUID = @guid,
                        UserID = @user_id,
                        ItemTypeID = @item_id,
                        AreaID = @area_id,
                        Title = @title,
                        Capacity = @capacity,
                        NumberOfBeds = @number_of_beds,
                        NumberOfBedrooms = @number_of_bedrooms,
                        NumberOfBathrooms = @number_of_bathrooms,
                        ExactAddress = @exact_address,
                        ApproximateAddress = @approximate_address,
                        Description = @description,
                        HostRules = @host_rule,
                        MinimumNights = @mimimum_night,
                        MaximumNights = @maximum_night
                        OUTPUT INSERTED.ID
                    WHERE ID = @item_id";
        }
        public string selectDataManagementForm()
        {
            return @"SELECT i.ID, i.Title, i.Capacity,a.Name as [Area],it.Name  as [Type]
                    FROM (( Areas AS a INNER JOIN Items as i ON a.ID = i.AreaID) INNER JOIN ItemTypes as it ON it.ID = i.ItemTypeID)
                    ORDER BY 1 ASC";
        }
        public string selectDataTraveler()
        {
            return @"SELECT i.Title, i.Capacity,a.Name as [Area],it.Name  as [Type]
                    FROM (( Areas AS a INNER JOIN Items as i ON a.ID = i.AreaID) INNER JOIN ItemTypes as it ON it.ID = i.ItemTypeID)
                    ORDER BY 1 ASC";
        }
        public string selectDataSearchForTraveler()
        {
            return @"SELECT i.Title, i.Capacity,a.Name as [Area],it.Name  as [Type]
                    FROM (( Areas AS a INNER JOIN Items as i ON a.ID = i.AreaID) INNER JOIN ItemTypes as it ON it.ID = i.ItemTypeID)
                    WHERE i.Title LIKE @title OR a.Name LIKE @area OR it.Name LIKE @type
                    ORDER BY 1 ASC";
        }
        public string selectDataItemAmentity()
        {
            return @"INSERT INTO ItemAmenities (GUID, ItemID, AmenityID) 
                         VALUES (@guid, @item_id, @amenity_id)";
        }
        public string updateItemAmentity()
        {
            return @"UPDATE ItemAmenities 
                    SET GUID = @guid, 
                        ItemID, @item_id,
                        AmenityID = @amenity_id 
                        WHERE ID = @id";
        }
        public string selectDataAmentity()
        {
            return "select ID ,Name from Amenities";
        }
        public string selectItemType()
        {
            return "SELECT ID, Name FROM ItemTypes";
        }
        public string selectArea()
        {
            return "SELECT ID, Name FROM Areas";
        }
        public string selectDataDistanceToAtrraction()
        {
            return @"SELECT att.Name AS [Attraction],
		                            a.Name AS [Area] , 
		                            iatt.Distance AS [Distance (Km)],
		                            iatt.DurationOnFoot AS [On Foot (minutes)],
		                            iatt.DurationByCar AS [By Car (minutes)]
                            FROM ((ItemAttractions AS iatt 
                            INNER JOIN Attractions AS att ON iatt.AttractionID = att.ID)
                            INNER JOIN Areas AS a ON att.AreaID = a.ID)
                            WHERE iatt.ItemID = @item_id";
        }
        public string selectItem()
        {
            return @"SELECT ID,UserID,ItemTypeID,AreaID,Title,
		                    Capacity,NumberOfBeds,NumberOfBedrooms,
		                    NumberOfBathrooms,ExactAddress,
		                    ApproximateAddress,Description,
		                    HostRules,MinimumNights,MaximumNights
                    FROM Items
                    WHERE ID = @item_id
                    ORDER BY 1 ASC";
        }

        public string selectAmenityOfItem()
        {
            return @"SELECT ID,ItemID,AmenityID
                    FROM ItemAmenities
                    WHERE ItemID = @item_id";
        }


    }
}
