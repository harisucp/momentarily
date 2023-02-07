SELECT * FROM c_good_property_value AS gpv
LEFT JOIN d_good_property AS gp ON gpv.good_property_id = gp.id
LEFT JOIN d_good_property_type AS gpt ON gp.[type_id] = gpt.id
LEFT JOIN c_good_property_value_definition AS gpvd ON gpv.property_value_definition_id = gpvd.id