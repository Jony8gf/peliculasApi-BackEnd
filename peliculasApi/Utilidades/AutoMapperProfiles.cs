using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NetTopologySuite.Geometries;
using peliculasApi.Entidades;
using peliculasApi.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace peliculasApi.Utilidades
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles(GeometryFactory geometryFactory)
        {
            CreateMap<Genero, GeneroViewModel>().ReverseMap();
            CreateMap<GeneroCreacionViewModel, Genero>();
            CreateMap<Actor, ActorViewModel>().ReverseMap();
            CreateMap<ActorCreacionViewModel, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<Cine, CineViewModel>()
                .ForMember(x => x.Latitud, dto => dto.MapFrom(campo => campo.Ubicacion.Y))
                .ForMember(x => x.Longitud, dto => dto.MapFrom(campo => campo.Ubicacion.X));
            CreateMap<CineCreacionViewModel, Cine>()
                .ForMember(x => x.Ubicacion, x => x.MapFrom(dto => geometryFactory.CreatePoint(new Coordinate(dto.Longitud, dto.Latitud))));
            CreateMap<PeliculaCreacionViewModel, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapearPeliculasGeneros))
                .ForMember(x => x.PeliculasCines, options => options.MapFrom(MapearPeliculasCines))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapearPeliculasActores));
            CreateMap<Pelicula, PeliculaViewModel>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapearPeliculasGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapearPeliculasActor))
                .ForMember(x => x.Cines, options => options.MapFrom(MapearPeliculasCines));
            CreateMap<IdentityUser, UsuarioViewModel>();


        }

        private List<PeliculasGeneros> MapearPeliculasGeneros(PeliculaCreacionViewModel peliculaCreacionViewModel, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();

            if (peliculaCreacionViewModel.GenerosIds == null)
                return resultado;

            foreach (var id in peliculaCreacionViewModel.GenerosIds)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }

            return resultado;
        }

        private List<PeliculasCines> MapearPeliculasCines(PeliculaCreacionViewModel peliculaCreacionViewModel, Pelicula pelicula)
        {
            var resultado = new List<PeliculasCines>();

            if (peliculaCreacionViewModel.CinesIds == null)
                return resultado;

            foreach (var id in peliculaCreacionViewModel.CinesIds)
            {
                resultado.Add(new PeliculasCines() { CineId = id });
            }

            return resultado;
        }

        private List<PeliculasActores> MapearPeliculasActores(PeliculaCreacionViewModel peliculaCreacionViewModel, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();

            if (peliculaCreacionViewModel.Actores == null)
                return resultado;

            foreach (var actor in peliculaCreacionViewModel.Actores)
            {
                resultado.Add(new PeliculasActores() { ActorId = actor.Id, Personaje = actor.Personaje });
            }

            return resultado;
        }

        private List<GeneroViewModel> MapearPeliculasGeneros(Pelicula pelicula, PeliculaViewModel peliculaVM)
        {
            var resultado = new List<GeneroViewModel>();

            if (pelicula.PeliculasGeneros != null)
            {
                foreach (var genero in pelicula.PeliculasGeneros)
                {
                    resultado.Add(new GeneroViewModel() { Id = genero.GeneroId, Nombre = genero.Genero.Nombre });
                }
            }

            return resultado;

        }

        private List<PeliculaActorViewModel> MapearPeliculasActor(Pelicula pelicula, PeliculaViewModel peliculaVM)
        {
            var resultado = new List<PeliculaActorViewModel>();

            if (pelicula.PeliculasActores != null)
            {
                foreach (var actor in pelicula.PeliculasActores)
                {
                    resultado.Add(new PeliculaActorViewModel()
                    {
                        Id = actor.ActorId,
                        Nombre = actor.Actor.Nombre,
                        Personaje = actor.Personaje,
                        Orden = actor.Orden,
                        Foto = actor.Actor.Foto
                    });
                }
            }

            return resultado;

        }


        private List<CineViewModel> MapearPeliculasCines(Pelicula pelicula, PeliculaViewModel peliculaVM)
        {
            var resultado = new List<CineViewModel>();

            if (pelicula.PeliculasCines != null)
            {
                foreach (var peliculasCines in pelicula.PeliculasCines)
                {
                    resultado.Add(new CineViewModel()
                    {
                        Id = peliculasCines.CineId,
                        Nombre = peliculasCines.Cine.Nombre,
                        Latitud = peliculasCines.Cine.Ubicacion.Y,
                        Longitud = peliculasCines.Cine.Ubicacion.X
                    });
                }
            }

            return resultado;

        }
    }
}
