function [m] = Sobel(mask)

if (mask == "H")
    m = [-1 -2 -1; 0 0 0; 1 2 1];
elseif (mask == "V") 
    m = [-1 0 1; -2 0 2; -1 0 1];

    
end

end