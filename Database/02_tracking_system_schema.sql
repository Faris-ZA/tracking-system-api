CREATE TYPE update_status AS ENUM ('New', 'Updated', 'Deleted');

-- Table: people
CREATE TABLE tblpeople (
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name TEXT, 
  phone TEXT,
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: tags
CREATE TABLE tbltags (
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  label TEXT,
  mac TEXT,
  update_status update_status DEFAULT 'New',
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Table: people_tag_association
CREATE TABLE tblpeople_tag_association (
  people_id INT REFERENCES tblpeople(id),
  tag_id INT REFERENCES tbltags(id),
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (people_id, tag_id)
 
);

--Table: venues
CREATE TABLE tblvenues (
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name TEXT,
  city TEXT,
  update_status update_status DEFAULT 'New',
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);	

--Table: floors
CREATE TABLE tblfloors (
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name TEXT,
  venue_id INT REFERENCES tblvenues(id),
  level INT,
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: zones
CREATE TABLE tblzones (
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  name TEXT,
  floor_id INT REFERENCES tblfloors(id),
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: zones_polygon_points
CREATE TABLE tblzones_polygon_points (
   zone_id INT REFERENCES tblzones(id),
   point_index INT,
   x INT,
   y INT,
   create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
   last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (zone_id, point_index)
);

--Table: last_positions
CREATE TABLE tbllast_position (
  people_id INT PRIMARY KEY REFERENCES tblpeople(id),
  venue_id INT REFERENCES tblvenues(id),
  floor_id INT REFERENCES tblfloors(id),
  zone_id INT REFERENCES tblzones(id),
  x INT,
  y INT,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: position_history
CREATE TABLE tblposition_history (	
  id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
  people_id INT REFERENCES tblpeople(id),
  venue_id INT REFERENCES tblvenues(id),
  floor_id INT REFERENCES tblfloors(id),
  zone_id INT REFERENCES tblzones(id),
  x INT,
  y INT,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);