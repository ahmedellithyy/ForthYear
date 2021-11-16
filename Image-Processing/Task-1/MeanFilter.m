function [m] = MeanFilter(h,w)

m= zeros(h,w)

for i=1:h
    for j=1:w
        m(i,j) = double(1/(h*w)); 
    end
end

end