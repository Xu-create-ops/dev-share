"use client";

import { useState } from "react";
import { ExternalLink, ThumbsUp, Bookmark } from "lucide-react";
import { Card, CardContent, CardFooter, CardHeader } from "@/components/ui/card";
import { Badge } from "@/components/ui/badge";
import { Button } from "@/components/ui/button";
import { 
  Tooltip,
  TooltipContent,
  TooltipProvider,
  TooltipTrigger,
} from "@/components/ui/tooltip";
import { Resource } from "@/lib/types";

interface ResourceCardProps {
  resource: Resource;
  onAction: (id: string, action: 'like' | 'bookmark') => void;
}

export default function ResourceCard({ resource, onAction }: ResourceCardProps) {
  const [isHovered, setIsHovered] = useState(false);

  return (
    <Card 
      className="h-full transition-all duration-300 hover:shadow-md flex flex-col group bg-card border-muted"
      onMouseEnter={() => setIsHovered(true)}
      onMouseLeave={() => setIsHovered(false)}
    >
      <CardHeader className="pb-2 pt-4 px-5">
        <div className="flex flex-wrap gap-2 mb-1">
          {resource.tags.map((tag) => (
            <Badge key={tag} variant="outline" className="text-xs font-medium">
              {tag}
            </Badge>
          ))}
        </div>
        <h3 className="text-xl font-semibold tracking-tight line-clamp-2 group-hover:text-primary transition-colors">{resource.title}</h3>
      </CardHeader>
      
      <CardContent className="flex-1 p-5 pt-0">
        <p className="text-sm text-muted-foreground line-clamp-3">
          {resource.description}
        </p>
      </CardContent>
      
      <CardFooter className="p-5 pt-0 flex items-center justify-between">
        <div className="flex items-center gap-2">
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  size="sm"
                  variant={resource.isLiked ? "default" : "ghost"}
                  className={`h-8 px-3 ${resource.isLiked ? 'text-primary-foreground' : 'text-muted-foreground'} transition-all duration-200`}
                  onClick={() => onAction(resource.id, 'like')}
                >
                  <ThumbsUp className={`mr-1 h-4 w-4 ${resource.isLiked ? 'fill-current' : ''}`} />
                  <span>{resource.likes}</span>
                </Button>
              </TooltipTrigger>
              <TooltipContent side="bottom">
                <p>{resource.isLiked ? 'Unlike' : 'Like'} this resource</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
          
          <TooltipProvider>
            <Tooltip>
              <TooltipTrigger asChild>
                <Button
                  size="sm"
                  variant={resource.isBookmarked ? "default" : "ghost"}
                  className={`h-8 px-3 ${resource.isBookmarked ? 'text-primary-foreground' : 'text-muted-foreground'} transition-all duration-200`}
                  onClick={() => onAction(resource.id, 'bookmark')}
                >
                  <Bookmark className={`h-4 w-4 ${resource.isBookmarked ? 'fill-current' : ''}`} />
                  <span className="sr-only">Bookmark</span>
                </Button>
              </TooltipTrigger>
              <TooltipContent side="bottom">
                <p>{resource.isBookmarked ? 'Remove bookmark' : 'Bookmark'} this resource</p>
              </TooltipContent>
            </Tooltip>
          </TooltipProvider>
        </div>
        
        <Button 
          size="sm" 
          variant="outline" 
          className="transition-all duration-200 hover:bg-primary hover:text-primary-foreground"
          asChild
        >
          <a href={resource.url} target="_blank" rel="noopener noreferrer" className="flex items-center">
            <span className="mr-2">View</span>
            <ExternalLink className="h-4 w-4" />
          </a>
        </Button>
      </CardFooter>
    </Card>
  );
}