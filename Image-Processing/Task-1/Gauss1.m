function [m] = Gauss1(size,sig)

m = zeros(size,size);
t = floor(double(size) / 2);
for i= -t:t
    for j= -t:t
        tmp = exp(-double(((i^2)+(j^2)))/double((2*(sig^2))));
        m(i + t + 1, j + t + 1) = (1/(2*pi*(sig^2)))*tmp; 
    end
end



end