CREATE TYPE update_status AS ENUM ('New', 'Updated', 'Deleted');

-- Table: people
CREATE TABLE tblpeople (
  id SERIAL PRIMARY KEY,
  name TEXT, 
  phone TEXT(15),
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: tags
CREATE TABLE tbltags (
  id SERIAL PRIMARY KEY,	
  label TEXT,
  mac TEXT,
  update_status update_status DEFAULT 'New',
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

-- Table: people_tag_association
CREATE TABLE tblpeople_tag_association (
  people_id INT REFERENCES tblpeople(id) ON DELETE CASCADE,
  tag_id INT REFERENCES tbltags(id) ON DELETE CASCADE,
  PRIMARY KEY (people_id, tag_id),
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
 
);

--Table: venues
CREATE TABLE tblvenues (
  id SERIAL PRIMARY KEY,
  name TEXT,
  city TEXT,
  update_status update_status DEFAULT 'New',
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);	

--Table: floors
CREATE TABLE tblfloors (
  id SERIAL PRIMARY KEY,
  name TEXT,
  venue_id INT REFERENCES tblvenues(id) ON DELETE CASCADE,
  level INT,
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: zones
CREATE TABLE tblzones (
  id SERIAL PRIMARY KEY,
  name TEXT,
  floor_id INT REFERENCES tblfloors(id) ON DELETE CASCADE,
  update_status update_status DEFAULT 'New',
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: zones_polygon_points
CREATE TABLE tblzones_polygon_points (
   zone_id INT REFERENCES tblzones(id) ON DELETE CASCADE,
   point_index INT,
   x INT,
   y INT,
   create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
   last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
   PRIMARY KEY (zone_id, point_index)
);

--Table: last_positions
CREATE TABLE tbllast_position (
  people_id INT PRIMARY KEY REFERENCES tblpeople(id) ON DELETE CASCADE,
  venue_id INT REFERENCES tblvenues(id) ON DELETE CASCADE,
  floor_id INT REFERENCES tblfloors(id) ON DELETE CASCADE,
  zone_id INT REFERENCES tblzones(id) ON DELETE CASCADE,
  x INT,
  y INT,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP,
  last_update TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);

--Table: position_history
CREATE TABLE tblposition_history (	
  id SERIAL PRIMARY KEY,
  people_id INT REFERENCES tblpeople(id) ON DELETE CASCADE,
  venue_id INT REFERENCES tblvenues(id) ON DELETE CASCADE,
  floor_id INT REFERENCES tblfloors(id) ON DELETE CASCADE,
  zone_id INT REFERENCES tblzones(id) ON DELETE CASCADE,
  x INT,
  y INT,
  create_date TIMESTAMP WITHOUT TIME ZONE DEFAULT CURRENT_TIMESTAMP
);